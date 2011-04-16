using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = FantasyEngineData.Items.Inventory;

namespace FantasyEngineContentPipelineExt
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class InventoryContentWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            //output.Write(value.Name);
            //output.WriteObject<List<string>>(value.ArmorList);
            output.WriteObject<List<string>>(value.WeaponList);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(InventoryContentReader).AssemblyQualifiedName;
        }
    }
}
