﻿using Fushigi.util;
using ImGuiNET;
using System.Numerics;

namespace Fushigi.ui.widgets
{
    /// <summary>
    /// A widget for displaying a given directory path with a button to select one. 
    /// </summary>
    internal class PathSelector
    {
        public static bool Show(string label, ref string path, bool isValid = true)
        {
            bool edited = false;
            bool clicked;
            //Ensure path isn't null for imgui
            if (path == null)
                path = "";

            //Validiate directory
            if (!System.IO.Directory.Exists(path))
                isValid = false;

            ImGui.BeginTable("path", 2, ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable);
            {
                ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed, 150.0f * UserSettings.GetUiScale());
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                    edited = false;

                    ImGui.Text(label);

                    ImGui.TableNextColumn();

                    ImGui.PushItemWidth(ImGui.GetColumnWidth() - 100 * UserSettings.GetUiScale());

                    if (!isValid)
                    {
                        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.5f, 0, 0, 1));
                        ImGui.InputText($"##{label}", ref path, 500, ImGuiInputTextFlags.ReadOnly);
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0.5f, 0, 1));
                        ImGui.InputText($"##{label}", ref path, 500, ImGuiInputTextFlags.ReadOnly);
                        ImGui.PopStyleColor();
                    }

                    if (ImGui.BeginPopupContextItem($"{label}_clear", ImGuiPopupFlags.MouseButtonRight))
                    {
                        if (ImGui.MenuItem("Clear"))
                        {
                            path = "";
                            edited = true;
                        }
                        ImGui.EndPopup();
                    }

                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    clicked = ImGui.Button($"Select##{label}");

                ImGui.EndTable();
            }

            if (clicked)
            {
                var dialog = new FolderDialog();
                if (dialog.ShowDialog())
                {
                    path = dialog.SelectedPath;
                    return true;
                }
            }
            return edited;
        }
    }
}
