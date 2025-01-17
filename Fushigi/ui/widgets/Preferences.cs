﻿using Fushigi.gl;
using Fushigi.param;
using Fushigi.ui.helpers;
using Fushigi.ui.modal;
using Fushigi.util;
using ImGuiNET;
using Silk.NET.OpenGL;
using System.Numerics;

namespace Fushigi.ui.widgets
{
    class Preferences
    {
        static readonly Vector4 errCol = new Vector4(1f, 0, 0, 1);
        static bool romfsTouched = false;
        static bool modRomfsTouched = false;
        static float uiScale = UserSettings.GetUiScale();

        public static void Draw(ref bool continueDisplay, GLTaskScheduler glTaskScheduler,
            IPopupModalHost modalHost)
        {
            ImGui.SetNextWindowSize(new Vector2(700, 250), ImGuiCond.Once);
            if (ImGui.Begin("Preferences", ImGuiWindowFlags.NoDocking))
            {
                ImGui.SetWindowFontScale(UserSettings.GetUiScale());

                var romfs = UserSettings.GetRomFSPath();
                var mod = UserSettings.GetModRomFSPath();
                var useGameShaders = UserSettings.UseGameShaders();
                var useAstcTextureCache = UserSettings.UseAstcTextureCache();

                ImGui.Indent();


                if (PathSelector.Show(
                    "RomFS Game Path",
                    ref romfs,
                    RomFS.IsValidRoot(romfs))
                    )
                {
                    romfsTouched = true;
 
                    UserSettings.SetRomFSPath(romfs);

                    if (!RomFS.IsValidRoot(romfs))
                    {
                        return;
                    }

                    Task.Run(async () =>
                    {
                        await ProgressBarDialog.ShowDialogForAsyncAction(modalHost,
                        $"Preloading Thumbnails",
                        async (p) =>
                        {
                            await glTaskScheduler.Schedule(gl => RomFS.SetRoot(romfs, gl));
                        });

                        ChildActorParam.Load();

                        /* if our parameter database isn't set, set it */
                        if (!ParamDB.sIsInit)
                        {
                            await MainWindow.LoadParamDBWithProgressBar(modalHost);
                        }
                    });
                    
                }

                Tooltip.Show("The game files which are stored under the romfs folder.");

                if (romfsTouched && !RomFS.IsValidRoot(romfs))
                {
                    ImGui.TextColored(errCol,
                        "The path you have selected is invalid. Please select a RomFS path that contains BancMapUnit, Model, and Stage.");
                }

                if (PathSelector.Show("Save Directory", ref mod, !string.IsNullOrEmpty(mod)))
                {
                    modRomfsTouched = true;

                    UserSettings.SetModRomFSPath(mod);
                }   

                Tooltip.Show("The save output where to save modified romfs files");

                if (modRomfsTouched && string.IsNullOrEmpty(mod))
                {
                    ImGui.TextColored(errCol,
                        "The path you have selected is invalid. Directory must not be empty.");
                }

                if (ImGui.Checkbox("Use Game Shaders", ref useGameShaders))
                {
                    UserSettings.SetGameShaders(useGameShaders);
                }

                Tooltip.Show("Displays models using the shaders present in the game. This may cause a performance drop but will look more visually accurate.");

                if (ImGui.Checkbox("Use Astc Texture Cache", ref useAstcTextureCache))
                {
                    UserSettings.SetAstcTextureCache(useAstcTextureCache);
                }

                Tooltip.Show("Saves ASTC textures to disk which takes up disk space, but improves loading times and ram usage significantly.");

                ImGui.SliderFloat("Ui Scale", ref uiScale, 0.25f, 4.0f);

                ImGui.SameLine();


                if (ImGui.Button("Apply Ui Scale"))
                {
                    ImGui.GetStyle().ScaleAllSizes(1.0f / UserSettings.GetUiScale());
                    UserSettings.SetUiScale(uiScale);
                    ImGui.GetStyle().ScaleAllSizes(uiScale);

                }

                ImGui.Unindent();

                if (ImGui.Button("Close"))
                {
                    continueDisplay = false;
                }

                ImGui.End();
            }
        }
    }
}
