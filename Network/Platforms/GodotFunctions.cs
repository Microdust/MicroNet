#if GODOT
using Godot;

namespace MicroNet.Network
{
    public unsafe partial class OutgoingMessage
    {
        private void WriteInternalVector2(Vector2 vector)
        {
            WriteInternalFloat(vector.x);
            WriteInternalFloat(vector.y);
        }

        private void WriteInternalVector3(Vector3 vector)
        {
            WriteInternalFloat(vector.x);
            WriteInternalFloat(vector.y);
            WriteInternalFloat(vector.z);
        }

        public void Write(Vector2 vector)
        {
            CheckAndBalance(64);

            WriteInternalFloat(vector.x);
            WriteInternalFloat(vector.y);           
        }

        public void Write(Vector3 vector)
        {
            CheckAndBalance(96);
            WriteInternalFloat(vector.x);
            WriteInternalFloat(vector.y);
            WriteInternalFloat(vector.z);
        }

        public void Write(Color color)
        {
            CheckAndBalance(128);
            WriteInternalFloat(color.r);
            WriteInternalFloat(color.g);
            WriteInternalFloat(color.b);
            WriteInternalFloat(color.a);               
        }

        public void Write(Quat quaternion)
        {
            CheckAndBalance(128);
            WriteInternalFloat(quaternion.x);
            WriteInternalFloat(quaternion.y);
            WriteInternalFloat(quaternion.z);
            WriteInternalFloat(quaternion.w);
        }

        public void Write(AABB box)
        {
            CheckAndBalance(192);
            WriteInternalVector3(box.Position);
            WriteInternalVector3(box.Size);
        }

        public void Write(Rect2 rect)
        {
            CheckAndBalance(128);
            WriteInternalVector2(rect.Position);
            WriteInternalVector2(rect.Size);
        }

        public void Write(Basis basis)
        {
            CheckAndBalance(288);
            WriteInternalVector3(basis.x);
            WriteInternalVector3(basis.y);
            WriteInternalVector3(basis.z);
        }

        // Not sure about this one.
        public void Write(Plane plane)
        {
            CheckAndBalance(128);
            WriteInternalFloat(plane.x);
            WriteInternalFloat(plane.y);
            WriteInternalFloat(plane.z);
            WriteInternalFloat(plane.D);
        }
    }

    public unsafe partial class IncomingMessage
    {
        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Color ReadColor()
        {
            return new Color(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Quat ReadQuaternion()
        {
            return new Quat(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public AABB ReadAABBox()
        {
            return new AABB(ReadVector3(), ReadVector3());
        }

        public Rect2 ReadRect2()
        {
            return new Rect2(ReadVector2(), ReadVector2());
        }

        public Basis ReadBasis()
        {
            return new Basis(ReadVector3(), ReadVector3(), ReadVector3());
        }

        public Plane ReadPlane()
        {
            return new Plane(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }
    }
}
#endif