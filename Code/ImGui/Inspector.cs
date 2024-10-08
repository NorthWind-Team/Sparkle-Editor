using System.Numerics;
using CopperDevs.Core.Utility;
using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Rendering;
using ImGuiNET;
using Sparkle_Editor.Code.Managers;
using Sparkle.CSharp.Logging;

namespace Sparkle_Editor.Code.ImGui;

[Window("Inspector", WindowOpen = true)]
public class Inspector : BaseWindow
{
    public override void WindowUpdate()
    {
        base.WindowUpdate();
        
        var ent = SelectingManager.SelectedEntity.LastOrDefault();
         
        if (ent == null) return;

        Vector3 tempPosition = ent.Position;
        Quaternion tempRotation = ent.Rotation;
        Vector3 tempScale = ent.Scale;
        string tempTag = ent.Tag;

        CopperImGui.HorizontalGroup(
            () => { CopperImGui.Button("Delete", SelectingManager.DeleteSelectedEntity); },
            () => { CopperImGui.Button("Copy", SelectingManager.CopySelectedEntity); },
            () => { CopperImGui.Button("Paste", SelectingManager.PasteSelectedEntity); },
            () => { CopperImGui.Button("Duplicate", SelectingManager.DuplicateSelectedEntity); });

        CopperImGui.Separator("Information");

        string ids = "";
        foreach (var entity in SelectingManager.SelectedEntity)
        {
            ids += $"{entity.Id}, ";
        }
        CopperImGui.Text($"Id: {ids}");
        
        if (SelectingManager.SelectedEntity.Count > 1)
        {
            CopperImGui.Text("Multi-edit not supported!");
        }
        else
        {
            ImGuiNET.ImGui.InputText("Tag", ref tempTag, 25);
        }
        
        CopperImGui.Space();
        
        ImGuiTreeNodeFlags flag = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;

        if (ImGuiNET.ImGui.TreeNodeEx("Transform", flag))
        {
            if (SelectingManager.SelectedEntity.Count > 1)
            {
                CopperImGui.Text("Multi-edit not supported!");
            }
            else
            {
                var eulerRotation = tempRotation.ToEulerAngles() * (180 / MathF.PI);

                Logger.Debug($"ent.Rotation: {ent.Rotation} | temp rotation: {tempRotation} | eulerRotation: {eulerRotation}");

                CopperImGui.DragValue("Position", ref tempPosition);
                CopperImGui.DragValue("Rotation", ref eulerRotation);
                CopperImGui.DragValue("Scale", ref tempScale);

                tempRotation = (eulerRotation * (MathF.PI / 180)).FromEulerAngles();
            }
            
            ImGuiNET.ImGui.TreePop();
        }
        
        ent.Position = tempPosition;
        ent.Rotation = tempRotation; 
        ent.Scale = tempScale;
        ent.Tag = tempTag;
        
        CopperImGui.Separator("Components");
        
        foreach (var comp in ent.GetComponents())
        {
            string compName = comp.ToString().Replace(comp.GetType().Namespace + ".", string.Empty);
            if (ImGuiNET.ImGui.TreeNodeEx(compName, flag))
            {
                if (SelectingManager.SelectedEntity.Count > 1)
                {
                    CopperImGui.Text("Multi-edit not supported!");
                }
                else
                {
                    Vector3 offsetPos = comp.OffsetPos;
                    
                    CopperImGui.DragValue("Offset", ref offsetPos);
                
                    comp.OffsetPos = offsetPos;
                }
                
                ImGuiNET.ImGui.TreePop();
            }
        }
    }
}