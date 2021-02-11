using System.ComponentModel;

namespace MemoryFilter.Settings {

    public enum OverrideStrategy {
        [Description("Target file is removed and replaced with a file from source directory")]
        OverrideTarget,

        [Description("Do not copy/move source file")]
        Skip,

        [Description("Add a suffix to a source file, e.g. img567_443")]
        GiveAnewNameToSourceFile,

        [Description("Source file will be removed")]
        RemoveSourceFile
    }

}