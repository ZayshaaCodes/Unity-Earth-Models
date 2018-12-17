using UnityEngine;

public class PlaneObject : MonoBehaviour
{
    public (Vector3[] positions, Quaternion[] rotations) GetPointsOnLine(Vector3 startPointPosition,Vector3 endPointPosition, float instanceSpacing, bool useFixedSpacing = false)
    {
        Vector3d starpos = ProjectToPlanarSurface(startPointPosition);
        Vector3d endpos = ProjectToPlanarSurface(endPointPosition);

        Vector3d delta = endpos - starpos;
        Vector3d dir = delta.normalized;
        double len = delta.magnitude;

        int c = Mathf.CeilToInt((float) (len / instanceSpacing));

        double step = useFixedSpacing ? len / c : instanceSpacing;

        Vector3[] pnts = new Vector3[c];
        Quaternion[] rots = new Quaternion[c];

        for (int i = 0; i < c; i++)
        {
            pnts[i] = starpos + dir * (step * i);
            rots[i] = Quaternion.LookRotation(dir, transform.up);
        }


        return (pnts, rots);

    }

    public Vector3d ProjectToPlanarSurface(Vector3 worldPoint)
    {
        double d = Vector3d.Dot(transform.up, worldPoint);
        Vector3d p = (Vector3d)worldPoint - (Vector3d)transform.up * d;
        return p;
    }
}