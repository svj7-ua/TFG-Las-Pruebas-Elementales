using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{

    public enum RoomType
    {
        none,
        normalRoom,
        shopRoom,
        bossRoom,
        startRoom,
        hallway,
        door,
        N_door,
        E_door,
        S_door,
        W_door
    }

    public class Room {

        public RectInt bounds;
        private int exits_needed;

        public bool isStartRoom;

        public bool isLeaf;

        public bool isBossRoom;
        public bool isShopRoom;

        public GameObject roomPrefab;

        private bool[] exits = new bool[4];     // [up, right, down, left] or [N, E, S, W]

        public Room (Vector2Int size, Vector2Int position) {
            bounds = new RectInt(position, size);
            exits_needed = 0;
            roomPrefab = null;
            isLeaf = false;
            isBossRoom = false;
            isShopRoom = false;
            isStartRoom = false;
        }

        // Check if the room intersects with another room, making sure the rooms don't overlap and have at least a space of 1 unit between them
        public static bool Intersects(Room room1, Room room2) {

            RectInt resizedRoom = new RectInt(room2.bounds.xMin - 1, room2.bounds.yMin - 1, room2.bounds.width + 2, room2.bounds.height + 2);

            return resizedRoom.Overlaps(room1.bounds);         

        }

        public void AddExit() {
            exits_needed++;
        }

        public void AddExit(int amount) {
            exits_needed += amount;
        }

        public int GetExitsNeeded() {
            return exits_needed;
        }



        public void SetExit(RectInt end) {
            
            int x = end.x - bounds.x;
            int y = end.y - bounds.y;
            // [0] is north, [1] is east, [2] is south, [3] is west

            if(Math.Abs(x) > Math.Abs(y)) {
                // Horizontal axis is more important
                if(x > 0) {
                    exits[1] = true;
                } else {
                    exits[3] = true;
                }
            } else {
                // Vertical axis is more important
                if(y > 0) {
                    exits[2] = true;
                } else {
                    exits[0] = true;
                }
            }

        }

        public int DEBUG_CountExits() {
            int count = 0;
            foreach(bool exit in exits) {
                if(exit) {
                    count++;
                }
            }
            return count;
        }

        public void SetExit(int index, bool value) {
            exits[index] = value;
        }

        public bool GetExit(int index) {
            return exits[index];
        }

        public bool[] GetExits() {
            return exits;
        }

    }

    public class Vertex : IEquatable<Vertex> {
        public Vector3 Position { get; private set; }

        public Vertex() {

        }

        public Vertex(Vector3 position) {
            Position = position;
        }

        public override bool Equals(object obj) {
            if (obj is Vertex v) {
                return Position == v.Position;
            }

            return false;
        }

        public bool Equals(Vertex other) {
            return Position == other.Position;
        }

        public override int GetHashCode() {
            return Position.GetHashCode();
        }
    }

    public class Vertex<T> : Vertex {
        public T Item { get; private set; }

        public Vertex(T item) {
            Item = item;
        }

        public Vertex(Vector3 position, T item) : base(position) {
            Item = item;
        }
    }

    public class Edge : IEquatable<Edge> {
        public Vertex<Room> U { get; set; }
        public Vertex<Room> V { get; set; }

        public bool isBad;

        public Edge() {

        }

        public Edge(Vertex<Room> u, Vertex<Room> v) {
            U = u;
            V = v;
        }

        public float CalculateDistance(){
            return Vector3.Distance(U.Position, V.Position);
        }

        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                && (left.V == right.U || left.V == right.V);
        }

        public static bool operator !=(Edge left, Edge right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }
}
