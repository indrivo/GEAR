namespace GR.Entities.Abstractions.Constants
{
    public static class FormeoControls
    {
        public static FormControl Select = new FormControl
        {
            Tag = "select",
            Id = "custom-select",
            Icon = "select",
            Group = "common",
            Type = string.Empty
        };

        public static FormControl Text = new FormControl
        {
            Tag = "input",
            Id = "text-input",
            Icon = "text-input",
            Group = "common",
            Type = "text"
        };

        public static FormControl Number = new FormControl
        {
            Tag = "input",
            Id = "number",
            Icon = "hash",
            Group = "common",
            Type = "number"
        };

        public static FormControl CheckBox = new FormControl
        {
            Tag = "input",
            Id = "checkbox",
            Icon = "checkbox",
            Group = "common",
            Type = "checkbox"
        };


        public static FormControl Date = new FormControl
        {
            Tag = "input",
            Id = "date-input",
            Icon = "calendar",
            Group = "common",
            Type = "date"
        };
    }

    public class FormControl
    {
        public string Tag { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Icon { get; set; }
    }
}
