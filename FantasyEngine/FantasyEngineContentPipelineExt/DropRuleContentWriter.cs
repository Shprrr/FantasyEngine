using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using FantasyEngineData;

using TWrite = FantasyEngineData.Drop.DropRule;

namespace FantasyEngineContentPipelineExt
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class DropRuleContentWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.LevelMinimum);
            output.Write(value.LevelMaximum);
            output.Write(value.Gold);
            output.WriteObject<List<string>>(value.TreasureRef);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(Drop.DropRuleContentReader).AssemblyQualifiedName;
        }
    }
}
