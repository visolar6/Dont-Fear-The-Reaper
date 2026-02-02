using Nautilus.Options.Attributes;

namespace DontFearTheReaper
{
    [Menu("Dont Fear The Reaper")]
    public class Options : Nautilus.Json.ConfigFile
    {
        [Toggle(LabelLanguageId = "Options.GlowingEyes", TooltipLanguageId = "Options.GlowingEyes.Tooltip")]
        public bool GlowingEyes = true;

        [Slider(LabelLanguageId = "Options.SpeedMultiplier", TooltipLanguageId = "Options.SpeedMultiplier.Tooltip", Min = 1f, Max = 3f, DefaultValue = 1.5f, Step = 0.01f, Format = "{0:P0}")]
        public float SpeedMultiplier = 1.5f;

        [Slider(LabelLanguageId = "Options.RoarIntensity", TooltipLanguageId = "Options.RoarIntensity.Tooltip", Min = 1f, Max = 3f, DefaultValue = 1.5f, Step = 0.01f, Format = "{0:F2}")]
        public float RoarIntensity = 1.5f;

        [Slider(LabelLanguageId = "Options.AggressionMultiplier", TooltipLanguageId = "Options.AggressionMultiplier.Tooltip", Min = 1f, Max = 3f, DefaultValue = 1.5f, Step = 0.01f, Format = "{0:F2}")]
        public float AggressionMultiplier = 1.5f;
    }
}
