namespace Loupedeck.WorldClock.Commands;

public class ClockCommand : ActionEditorCommand
{
    public ClockCommand() : base()
    {
        this.DisplayName = "World Clock";
        this.Description = "Shows a world clock";
        this.GroupName = "Clock";
        
        ArgumentNullException.ThrowIfNull(this.ActionEditor);

        this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "displayName", labelText: "Display Name",
            description: "Extra text to display above the time"));
        this.ActionEditor.AddControlEx(
            new ActionEditorListbox(name: "timezone", labelText: "Timezone", description: "The Timezone")
                .SetRequired());

        // Set default values
        this.ActionEditor.ControlsStateRequested += (_, e) =>
        {
            if (e.ActionEditorState.GetControlValue("timezone").IsNullOrEmpty())
            {
                e.ActionEditorState.SetValue("timezone", "12");
            }
        };
        
        this.ActionEditor.ListboxItemsRequested += (_, e) =>
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones();
            for (var i = 0; i < timezones.Count; i++)
            {
                var tzInfo = timezones[i];
                e.AddItem(i.ToString(), tzInfo.DisplayName, tzInfo.DisplayName);
            }
        };
        
        this.ActionEditor.Finished += (_, e) =>
        {
            if (!e.IsCanceled)
            {
                this.ActionImageChanged();
            }
        };

        ClockTimer.ClockTick += (_, _) => this.ActionImageChanged();
    }

    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight)
    {
        ArgumentNullException.ThrowIfNull(actionParameters);
        
        var tzIndex = actionParameters.GetInt32("timezone");
        var timezone = TimeZoneInfo.GetSystemTimeZones()[tzIndex];
        
        // Get the current UTC time
        DateTime utcNow = DateTime.UtcNow;
            
        // Convert the current UTC time to the specified time zone
        DateTime timeInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timezone);
        
        using var bitmapBuilder = new BitmapBuilder(imageWidth, imageHeight);
        
        if (actionParameters.TryGetString("displayName", out var displayName))
        {
            bitmapBuilder.DrawText(displayName, 0, 5, imageWidth, 10, fontSize: 14);
        }
        bitmapBuilder.DrawText(timeInTimeZone.ToString("HH:mm"), 0, 20, imageWidth, imageHeight - 20, fontSize: 25);
        return bitmapBuilder.ToImage();
    }
}