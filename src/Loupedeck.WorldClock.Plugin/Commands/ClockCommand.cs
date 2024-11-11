namespace Loupedeck.WorldClock.Commands;

using Helpers;

public class ClockCommand : ActionEditorCommand
{
    private static readonly String[] Formats = ["12", "24"];

    public ClockCommand()
    {
        this.DisplayName = "World Clock";
        this.Description = "Shows a world clock";
        this.GroupName = "Clock";

        ArgumentNullException.ThrowIfNull(this.ActionEditor);

        this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "displayName", labelText: "Display Name",
            description: "Optional: Extra text to display above the time."));

        this.ActionEditor.AddControlEx(
            new ActionEditorListbox(name: "timezone", labelText: "Timezone", description: "The Timezone")
                .SetRequired());

        this.ActionEditor.AddControlEx(
            new ActionEditorListbox(name: "format", labelText: "Display Format",
                    description: "The format to display the time in.")
                .SetRequired());

        // Set default values
        this.ActionEditor.ControlsStateRequested += (_, e) =>
        {
            if (e.ActionEditorState.GetControlValue("timezone").IsNullOrEmpty())
            {
                e.ActionEditorState.SetValue("timezone", "12");
            }

            if (e.ActionEditorState.GetControlValue("format").IsNullOrEmpty())
            {
                e.ActionEditorState.SetValue("format", "1"); // Default to 24-hour time
            }
        };

        this.ActionEditor.ListboxItemsRequested += (_, e) =>
        {
            switch (e.ControlName)
            {
                case "timezone":
                {
                    var timezones = TimeZoneInfo.GetSystemTimeZones();
                    for (var i = 0; i < timezones.Count; i++)
                    {
                        var tzInfo = timezones[i];
                        e.AddItem(i.ToString(), tzInfo.DisplayName, tzInfo.DisplayName);
                    }

                    break;
                }
                case "format":
                {
                    for (var i = 0; i < Formats.Length; i++)
                    {
                        var format = Formats[i];
                        e.AddItem(i.ToString(), format, format);
                    }

                    break;
                }
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

    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth,
        Int32 imageHeight)
    {
        ArgumentNullException.ThrowIfNull(actionParameters);

        var formatIndex = actionParameters.GetInt32("format");
        var tzIndex = actionParameters.GetInt32("timezone");
        var timezone = TimeZoneInfo.GetSystemTimeZones()[tzIndex];

        // Get the current UTC time
        DateTime utcNow = DateTime.UtcNow;

        // Convert the current UTC time to the specified time zone
        DateTime timeInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timezone);

        using var bitmapBuilder = new BitmapBuilder(imageWidth, imageHeight);

        var yOffset = 0;
        if (actionParameters.TryGetString("displayName", out var displayName))
        {
            displayName = displayName.Trim();
            if (!displayName.IsNullOrEmpty())
            {
                bitmapBuilder.DrawText(displayName, 0, 5, imageWidth, 10, fontSize: 14);
                yOffset += 20;
            }
        }

        switch (formatIndex)
        {
            case 0:
                // 12
                bitmapBuilder.DrawText(timeInTimeZone.ToString("h:mm tt"), 0, yOffset, imageWidth, imageHeight - yOffset,
                    fontSize: 25, lineHeight: 20);
                break;
            case 1:
                // 24
                bitmapBuilder.DrawText(timeInTimeZone.ToString("HH:mm"), 0, yOffset, imageWidth, imageHeight - yOffset,
                    fontSize: 25);
                break;
        }

        return bitmapBuilder.ToImage();
    }
}