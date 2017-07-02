using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cubil_Face_Manager_Helper 
{


    //Helper For Creating Fracture Quads and Switching Vertice Information based on Direction
    public static void CreateOpposingFracturesHelp(List<Quad> QuadList, Vector3 normalDir, Direction FaceDirection, Quad innerQuad, Color _Color,
        Vertex vert0, Vertex sideVert0, Vertex oppVert0,
        Vertex vert1, Vertex sideVert1, Vertex oppVert1,
        Vertex vert2, Vertex sideVert2, Vertex oppVert2,
        Vertex vert3, Vertex sideVert3, Vertex oppVert3) {

        if (Mathf.Abs(vert0.vertice.y - oppVert0.vertice.y) > 0 || Mathf.Abs(vert1.vertice.y - oppVert1.vertice.y) > 0)
        {
            Quad opp0 = Quad.create(innerQuad.centre, normalDir, FaceDirection, _Color);
            opp0.vertexPoints[0] = new Vertex(oppVert0.vertice, oppVert0.normal, oppVert0.centre);
            opp0.vertexPoints[1] = new Vertex(oppVert1.vertice, oppVert1.normal, oppVert1.centre);
            opp0.vertexPoints[2] = new Vertex(vert0.vertice, vert0.normal, vert0.centre);
            opp0.vertexPoints[3] = new Vertex(vert1.vertice, vert1.normal, vert1.centre);

            opp0.CalculateQuadArea();
            opp0.CalculateCentre();
            opp0.CalculateCentrePoints();

            //opp0.quadColor = customColors[0];
            QuadList.Add(opp0);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, opp0);
        }

        if (Mathf.Abs(vert1.vertice.x - sideVert1.vertice.x) > 0 || Mathf.Abs(vert3.vertice.x - sideVert3.vertice.x) > 0)
        {
            Quad side0 = Quad.create(innerQuad.centre, normalDir, FaceDirection, _Color);
            side0.vertexPoints[0] = new Vertex(vert1.vertice, vert1.normal, vert1.centre);
            side0.vertexPoints[1] = new Vertex(sideVert1.vertice, sideVert1.normal, sideVert1.centre);
            side0.vertexPoints[2] = new Vertex(vert3.vertice, vert3.normal, vert3.centre);
            side0.vertexPoints[3] = new Vertex(sideVert3.vertice, sideVert3.normal, sideVert3.centre);

            side0.CalculateQuadArea();
            side0.CalculateCentre();
            side0.CalculateCentrePoints();

            //side0.quadColor = customColors[1];
            QuadList.Add(side0);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, side0);
        }

        if (Mathf.Abs(vert2.vertice.y - oppVert2.vertice.y) > 0 || Mathf.Abs(vert3.vertice.y - oppVert3.vertice.y) > 0)
        {
            Quad opp1 = Quad.create(innerQuad.centre, normalDir, FaceDirection, _Color);
            opp1.vertexPoints[0] = new Vertex(vert2.vertice, vert2.normal, vert2.centre);
            opp1.vertexPoints[1] = new Vertex(vert3.vertice, vert3.normal, vert3.centre);
            opp1.vertexPoints[2] = new Vertex(oppVert2.vertice, oppVert2.normal, oppVert2.centre);
            opp1.vertexPoints[3] = new Vertex(oppVert3.vertice, oppVert3.normal, oppVert3.centre);

            opp1.CalculateQuadArea();
            opp1.CalculateCentre();
            opp1.CalculateCentrePoints();

            // opp1.quadColor = customColors[2];
            QuadList.Add(opp1);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, opp1);
        }

        if (Mathf.Abs(sideVert0.vertice.x - vert0.vertice.x) > 0 || Mathf.Abs(sideVert2.vertice.x - vert2.vertice.x) > 0)
        {
            Quad side1 = Quad.create(innerQuad.centre, normalDir, FaceDirection, _Color);
            side1.vertexPoints[0] = new Vertex(sideVert0.vertice, sideVert0.normal, sideVert0.centre);
            side1.vertexPoints[1] = new Vertex(vert0.vertice, vert0.normal, vert0.centre);
            side1.vertexPoints[2] = new Vertex(sideVert2.vertice, sideVert2.normal, sideVert2.centre);
            side1.vertexPoints[3] = new Vertex(vert2.vertice, vert2.normal, vert2.centre);

            side1.CalculateQuadArea();
            side1.CalculateCentre();
            side1.CalculateCentrePoints();

            //side1.quadColor = customColors[3];
            QuadList.Add(side1);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, side1);
        }
    }


}
