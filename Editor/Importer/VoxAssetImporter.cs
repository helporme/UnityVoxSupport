using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace VoxSupport.Editor.Importer
{
    [ScriptedImporter(150, "vox")]
    public class VoxAssetImporter : ScriptedImporter
    {
        public ImportModelOptions modelOptions = new ImportModelOptions();
        public ImportMaterialsOptions materialsOptions = new ImportMaterialsOptions();
        public ImportStats stats = new ImportStats();
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var obj = new GameObject();
            ctx.AddObjectToAsset("Main", obj);
            ctx.SetMainObject(obj);
            
            AddMesh(ctx, obj);
            AddMaterial(obj);
        }

        private void AddMesh(AssetImportContext ctx, GameObject obj)
        {
            var meshFilter = obj.AddComponent<MeshFilter>();
            var mesh = new Mesh {name = Path.GetFileNameWithoutExtension(ctx.assetPath) };

            var stopwatch = Stopwatch.StartNew();
            
            var vox = VoxImporter.Import<VoxMesh>(ctx.assetPath);
            stopwatch.Stop();
            stats.importTime = stopwatch.ElapsedMilliseconds / 1000f;
            
            stopwatch.Restart();
            vox.Recalculate();
            stats.convertTime = stopwatch.ElapsedMilliseconds / 1000f;

            stats.vertexCount = vox.Vertices.Count;
            stats.linkCount = vox.Links.Count;
            stats.faceCount = vox.Faces.Count(f => f.Vertices.Count > 0);
            
            if (modelOptions.transformMesh)
            {
                ApplyTransformToMesh(vox);
            }
            else
            {
                ApplyTransformToObj(obj, vox);
            }

            if (vox.Vertices.Count > ushort.MaxValue)
            {
                mesh.indexFormat = IndexFormat.UInt32;
            }
            
            vox.Write(mesh);
            vox.Clear();
            
            meshFilter.sharedMesh = mesh;
            
            ctx.AddObjectToAsset("Mesh", meshFilter.sharedMesh);
        }
        
        private void ApplyTransformToMesh(VoxMesh vox)
        {
            modelOptions.scale = Mathf.Max(modelOptions.scale, 0.00001f);

            Vector3 translate = Vector3.zero;
            
            if (modelOptions.moveToFloor)
            {
                translate = GetFloorPosition(vox);
            }
            
            Matrix4x4 trs = Matrix4x4.TRS(translate, Quaternion.identity, Vector3.one * modelOptions.scale);
            vox.Transform(trs);
            vox.Rotate(Matrix4x4.Rotate(GetTransformRotation()));
        }

        private void ApplyTransformToObj(GameObject obj, VoxMesh vox)
        {
            if (modelOptions.moveToFloor)
            {
                obj.transform.position = GetFloorPosition(vox);
            }

            obj.transform.rotation = GetTransformRotation();
            obj.transform.localScale *= modelOptions.scale;
        }

        private Quaternion GetTransformRotation()
        {
            return modelOptions.orientation switch
            {
                ImportModelOptions.Orientation.XZY => Quaternion.Euler(-90, 0, 0),
                ImportModelOptions.Orientation.ZYX => Quaternion.Euler(0, -90, 0),
                _ => Quaternion.identity
            };
        }

        private Vector3 GetFloorPosition(VoxMesh vox)
        {
            return Vector3.back * vox.CalculateBounds().min.z * modelOptions.scale;
        }

        private void AddMaterial(GameObject obj)
        {
            var meshRenderer = obj.AddComponent<MeshRenderer>();

            if (!materialsOptions.material)
            {
                materialsOptions.material = GetDefaultMaterial();
            }
            
            meshRenderer.sharedMaterial = materialsOptions.material;
        }

        private Material GetDefaultMaterial()
        {
            if (EditorPrefs.HasKey("VoxAssetImporterDefaultMaterial"))
            {
                return (Material)EditorUtility.InstanceIDToObject(EditorPrefs.GetInt("VoxAssetImporterDefaultMaterial"));
            }
            return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        }
    }
}