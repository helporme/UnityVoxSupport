using UnityEngine;

namespace VoxSupport
{
    [ExecuteInEditMode]
    public class _Presentor : MonoBehaviour
    {
        public string path = @"C:\Users\HelpOrMe\Desktop\VoxSupportPackage\Packages\VoxSupport\Resources\sample.vox";
        public bool update;
        
        public void Update()
        {
            if (!update)
            {
                return;
            }
            update = false;

            var vox = VoxImporter.Import<VoxMesh>(path);
            
            vox.Recalculate();
            vox.Transform(Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0)));
            vox.Write(GetComponent<MeshFilter>().sharedMesh);
        }
    }
}