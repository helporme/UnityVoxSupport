using Unity.Mathematics;
using VoxSupport.Utils;

namespace VoxSupport
{
    public partial struct VoxelFace
    {
        public void CalculateLinks()
        {
            if (Corners == 0)
            {
                return;
            }

            for (int cornerIndex = 0; cornerIndex < 4; cornerIndex++)
            {
                if (((1 << cornerIndex) & Corners) == 0)
                {
                    continue;
                }

                CalculateLeftLinks(cornerIndex);
                CalculateDownLinks(cornerIndex);
            }

            if ((FaceCorners.RightUp & Corners) != 0 && IsBackswingCorner())
            {
                CalculateCrossLinks();
            }
        }

        private void CalculateLeftLinks(int corner)
        {
            int sideCorner = FaceCorners.LinkX[corner];
            int axisIndex = Axises.x;
            bool isBackswingSideCorner = ((1 << sideCorner) & FaceCorners.LeftSide) != 0;
            CalculateLinks(corner, sideCorner, isBackswingSideCorner, axisIndex);
        }

        private void CalculateDownLinks(int corner)
        {
            int sideCorner = FaceCorners.LinkY[corner];
            int axisIndex = Axises.y;
            bool isBackswingSideCorner = ((1 << sideCorner) & FaceCorners.DownSide) != 0;
            CalculateLinks(corner, sideCorner, isBackswingSideCorner, axisIndex);
        }

        private void CalculateLinks(int corner, int sideCorner, bool isBackswingSideCorner, int axisIndex)
        {
            int edge = FaceCorners.Edges[corner][sideCorner];
            int cornerVert = CornerVertIndices[corner];
            int sideCornerVert = CornerVertIndices[sideCorner];

            if (isBackswingSideCorner && sideCornerVert != -1)
            {
                if (IsEdgeExist(edge))
                {
                    AddVerticesLink(new int2(corner, sideCorner), new int2(cornerVert, sideCornerVert));
                }
                return;
            }

            bool prevVoxelHasEdge = IsEdgeExist(edge);

            for (int d = Pos[axisIndex] - 1; d >= 0; d--)
            {
                int3 sidePos = Pos;
                sidePos[axisIndex] = d;
                Voxel sideVoxel = Mesh.Voxels[sidePos];

                if (sideVoxel.Color != Voxel.Color || (sideVoxel.Normals & (1 << FaceIndex)) == NormalIndices.None)
                {
                    return;
                }

                if (!IsEdgeExist(sidePos, sideVoxel, edge))
                {
                    int backswingCorner = FaceCorners.EdgeClosestBackswingCorners[edge];
                    if (prevVoxelHasEdge && (sideVoxel.Corners & (1 << (FaceIndex * 4 + backswingCorner))) != 0)
                    {
                        int backswingCornerVert = sideVoxel.CornerVertIndices[FaceIndex][backswingCorner];
                        AddVerticesLink(new int2(corner, sideCorner), new int2(cornerVert, backswingCornerVert));
                    }
                    return;
                }

                prevVoxelHasEdge = true;

                if ((sideVoxel.Corners & (1 << (FaceIndex * 4 + corner))) != 0)
                {
                    sideCornerVert = sideVoxel.CornerVertIndices[FaceIndex][corner];
                    AddVerticesLink(new int2(corner, sideCorner), new int2(cornerVert, sideCornerVert));
                    return;
                }

                if ((sideVoxel.Corners & (1 << (FaceIndex * 4 + sideCorner))) != 0)
                {
                    sideCornerVert = sideVoxel.CornerVertIndices[FaceIndex][sideCorner];
                    AddVerticesLink(new int2(corner, sideCorner), new int2(cornerVert, sideCornerVert));
                    return;
                }
            }
        }

        private bool IsEdgeExist(int edge)
        {
            return IsEdgeExist(Pos, Voxel, edge);
        }

        private bool IsEdgeExist(int3 pos, Voxel voxel, int edge)
        {
            int sideNormalIndex = FaceCorners.EdgePerpNormalIndices[FaceIndex][edge];
            if ((voxel.Normals & (1 << sideNormalIndex)) == NormalIndices.None)
            {
                Voxel sideVoxel = Mesh.Voxels[pos + NormalVectors.ForwardVectors[sideNormalIndex]];
                return sideVoxel.Color != Voxel.Color || (sideVoxel.Normals & (1 << FaceIndex)) == 0;
            }

            return true;
        }

        private void AddVerticesLink(int2 corners, int2 cornerVerts)
        {
            if (FaceCorners.IsPositiveLinkOrder[corners.x][corners.y])
            {
                if (corners.y == FaceCorners.Next[corners.x])
                {
                    Mesh.Links[cornerVerts.y] = cornerVerts.x;
                }
                else
                {
                    Mesh.Links[cornerVerts.x] = cornerVerts.y;
                }
            }
            else
            {
                if (corners.x == FaceCorners.Next[corners.y])
                {
                    Mesh.Links[cornerVerts.y] = cornerVerts.x;
                }
                else
                {
                    Mesh.Links[cornerVerts.x] = cornerVerts.y;
                }
            }
        }

        private bool IsBackswingCorner()
        {
            int3 rightPos = Pos + NormalVectors.RightVectors[FaceIndex];
            int3 upPos = Pos + NormalVectors.UpVectors[FaceIndex];

            return Mesh.Voxels.InBounds(rightPos) && Mesh.Voxels[rightPos].Color == Voxel.Color ||
                   Mesh.Voxels.InBounds(upPos) && Mesh.Voxels[upPos].Color == Voxel.Color;
        }

        private void CalculateCrossLinks()
        {
            int3 sidePos = Pos;
            int normal = 1 << FaceIndex;

            for (int x = Pos[Axises.x]; x >= 0; x--)
            {
                for (int y = Pos[Axises.y]; y >= 0; y--)
                {
                    sidePos[Axises.x] = x;
                    sidePos[Axises.y] = y;
                    Voxel sideVoxel = Mesh.Voxels[sidePos];

                    if (sideVoxel.Color == Voxel.Color && (sideVoxel.Normals & normal) != 0)
                    {
                        if (TryToCalculateCrossLink(sideVoxel))
                        {
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private bool TryToCalculateCrossLink(Voxel sideVoxel)
        {
            int[] sideVoxelCornerVertIndices = sideVoxel.CornerVertIndices[FaceIndex];

            if (sideVoxelCornerVertIndices == null)
            {
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                int sideVoxelCornerIndex = sideVoxelCornerVertIndices[FaceCorners.Prev[i]];
                if (sideVoxelCornerIndex != -1)
                {
                    Mesh.CrossLinks[CornerVertIndices[2]] = sideVoxelCornerIndex;
                    return true;
                }
            }

            return false;
        }
    }
}