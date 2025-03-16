using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;
using System;

public class Delaunay
{

    public class Triangle: IEquatable<Triangle>{
        public Vertex<Room> A_vertex{get; set;}
        public Vertex<Room> B_vertex;
        public Vertex<Room> C_vertex;
        public bool isBad;

        public Triangle(){
            A_vertex = null;
            B_vertex = null;
            C_vertex = null;
            isBad = false;
        }

        public Triangle(Vertex<Room> A, Vertex<Room> B, Vertex<Room> C){
            A_vertex = A;
            B_vertex = B;
            C_vertex = C;
            isBad = false;
        }

        public bool ContainsVertex(Vector3 vertex){
            return Vector3.Distance(vertex, A_vertex.Position) < 0.1f || Vector3.Distance(vertex, B_vertex.Position) < 0.1f || Vector3.Distance(vertex, C_vertex.Position) < 0.1f;
        }

        public bool CircumCircleContains(Vector3 point){
            Vector3 A = A_vertex.Position;
            Vector3 B = B_vertex.Position;
            Vector3 C = C_vertex.Position;

            float a_magnitude = A.sqrMagnitude;
            float b_magnitude = B.sqrMagnitude;
            float c_magnitude = C.sqrMagnitude;

            float circumX = (a_magnitude * (C.y - B.y) + b_magnitude * (A.y - C.y) + c_magnitude * (B.y - A.y)) / ((A.x * (C.y - B.y) + B.x * (A.y - C.y) + C.x * (B.y - A.y)));
            float circumY = (a_magnitude * (C.x - B.x) + b_magnitude * (A.x - C.x) + c_magnitude * (B.x - A.x)) / ((A.y * (C.x - B.x) + B.y * (A.x - C.x) + C.y * (B.x - A.x)));

            Vector3 circumcenter = new Vector3(circumX/2, circumY/2, 0);
            float circumRadius = Vector3.SqrMagnitude(A - circumcenter);
            float dist = Vector3.SqrMagnitude(point - circumcenter);

            return dist <= circumRadius;
        }

        // Returns true if both triangles share the same vertices (in any order)
        public static bool operator ==(Triangle left, Triangle right){
            return (left.A_vertex == right.A_vertex || left.A_vertex == right.B_vertex || left.A_vertex == right.C_vertex)
                && (left.B_vertex == right.A_vertex || left.B_vertex == right.B_vertex || left.B_vertex == right.C_vertex)
                && (left.C_vertex == right.A_vertex || left.C_vertex == right.B_vertex || left.C_vertex == right.C_vertex);
        }

        public static bool operator !=(Triangle left, Triangle right){
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Triangle t) {
                return this == t;
            }

            return false;
        }

        public bool Equals(Triangle t) {
            return this == t;
        }

        public override int GetHashCode() {
            return A_vertex.GetHashCode() ^ B_vertex.GetHashCode() ^ C_vertex.GetHashCode();
        }

    }

    public class Edge : Graphs.Edge{

        public Edge(Vertex<Room> u, Vertex<Room> v): base(u, v){
        }
        public static bool AlmostEqual(Edge left, Edge right){
            return Delaunay.AlmostEqual(left.U, right.U) && Delaunay.AlmostEqual(left.V, right.V) || Delaunay.AlmostEqual(left.U, right.V) && Delaunay.AlmostEqual(left.V, right.U);
        }

    }

    static bool AlmostEqual(float x, float y) {
        return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2
            || Mathf.Abs(x - y) < float.MinValue;
    }

    static bool AlmostEqual(Vertex left, Vertex right) {
        return AlmostEqual(left.Position.x, right.Position.x) && AlmostEqual(left.Position.y, right.Position.y);
    }

    public List<Vertex<Room>> vertices;
    public List<Edge> edges;
    public List<Triangle> triangles;

    public Delaunay(){
        edges = new List<Edge>();
        triangles = new List<Triangle>();
    }

    public static Delaunay Triangulate(List<Vertex<Room>> vertices_to_triangulate){
        Delaunay delaunay = new Delaunay();
        delaunay.vertices = vertices_to_triangulate;
        delaunay.Triangulate();

        return delaunay;
    }

    void Triangulate(){

        float minX = vertices[0].Position.x;
        float minY = vertices[0].Position.y;
        Room minRoom = vertices[0].Item;
        float maxX = minX;
        float maxY = minY;
        Room maxRoom = minRoom;

        foreach(Vertex<Room> vertex in vertices){
            if(vertex.Position.x < minX){
                minX = vertex.Position.x;
            }

            if(vertex.Position.y < minY){
                minY = vertex.Position.y;
            }

            if(vertex.Position.x > maxX){
                maxX = vertex.Position.x;
            }

            if(vertex.Position.y > maxY){
                maxY = vertex.Position.y;
            }
        }

        float diff_x = maxX - minX;
        float diff_y = maxY - minY;
        float deltaMax = Mathf.Max(diff_x, diff_y);

        Vertex<Room> p1 = new Vertex<Room>(new Vector2(minX - 1, minY - 1), null);
        Vertex<Room> p2 = new Vertex<Room>(new Vector2(minX - 1, maxY + deltaMax), null);
        Vertex<Room> p3 = new Vertex<Room>(new Vector2(maxX + deltaMax, minY - 1), null);

        triangles.Add(new Triangle(p1, p2, p3));

        foreach(Vertex<Room> vertex in vertices){
            List<Edge> polygon = new List<Edge>();

            foreach(Triangle triangle in triangles){
                if(triangle.CircumCircleContains(vertex.Position)){
                    triangle.isBad = true;
                    polygon.Add(new Edge(triangle.A_vertex, triangle.B_vertex));
                    polygon.Add(new Edge(triangle.B_vertex, triangle.C_vertex));
                    polygon.Add(new Edge(triangle.C_vertex, triangle.A_vertex));
                }
            }

            triangles.RemoveAll(t => t.isBad);

            for(int i=0; i<polygon.Count; i++){
                for(int j=i+1; j<polygon.Count; j++){
                    if(Edge.AlmostEqual(polygon[i], polygon[j])){
                        polygon[i].isBad = true;
                        polygon[j].isBad = true;
                    }
                }
            }

            polygon.RemoveAll(e => e.isBad);

            foreach(Edge edge in polygon){
                triangles.Add(new Triangle(edge.U, edge.V, vertex));
            }

        }

        triangles.RemoveAll(t => t.ContainsVertex(p1.Position) || t.ContainsVertex(p2.Position) || t.ContainsVertex(p3.Position));

        HashSet<Edge> edges_set = new HashSet<Edge>();

        foreach(Triangle triangle_aux in triangles){

            // Adds the edge A-B
            if(edges_set.Add(new Edge(triangle_aux.A_vertex, triangle_aux.B_vertex)))       // If the edge is not already in the set, add it to the set and to the list
                edges.Add(new Edge(triangle_aux.A_vertex, triangle_aux.B_vertex));
            
            // Adds the edge B-C
            if(edges_set.Add(new Edge(triangle_aux.B_vertex, triangle_aux.C_vertex)))       // If the edge is not already in the set, add it to the set and to the list
                edges.Add(new Edge(triangle_aux.B_vertex, triangle_aux.C_vertex));

            // Adds the edge C-A
            if(edges_set.Add(new Edge(triangle_aux.C_vertex, triangle_aux.A_vertex)))       // If the edge is not already in the set, add it to the set and to the list
                edges.Add(new Edge(triangle_aux.C_vertex, triangle_aux.A_vertex));
        }

    }

}
