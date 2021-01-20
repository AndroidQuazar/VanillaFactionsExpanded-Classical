using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VFERomans
{
    [StaticConstructorOnStartup]
    internal  static class VFERoman_TextureLoader
    {
        public static readonly Texture2D iconCustomize = ContentFinder<Texture2D>.Get("GUI/customizebutton");
        public static readonly Texture2D pawnCircle = ContentFinder<Texture2D>.Get("GUI/unitCircle");

    }
}
