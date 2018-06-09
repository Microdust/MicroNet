#if UNITY
using UnityEngine;
namespace MicroNet.Network
{
    public partial class OutgoingMessage
    {
        public void WriteVector2(Vector2 vector)
        {
            WriteFloat(vector.x);
            WriteFloat(vector.y);
        }

        public void WriteVector3(Vector3 vector)
        {
            WriteFloat(vector.x);
            WriteFloat(vector.y);
            WriteFloat(vector.z);
        }

        public void WriteVector4(Vector4 vector)
        {
            WriteFloat(vector.x);
            WriteFloat(vector.y);
            WriteFloat(vector.z);
            WriteFloat(vector.w);
        }

        public void WriteColorAlpha(Color color)
        {
            WriteFloat(color.r);
            WriteFloat(color.g);
            WriteFloat(color.b);
            WriteFloat(color.a);
        }

        public void WriteColor(Color color)
        {
            WriteFloat(color.r);
            WriteFloat(color.g);
            WriteFloat(color.b);
        }

        public void WriteQuaternion(Quaternion quaternion)
        {
            WriteFloat(quaternion.x);
            WriteFloat(quaternion.y);
            WriteFloat(quaternion.z);
            WriteFloat(quaternion.w);
        }

        public void WriteRect(Rect rect)
        {
            WriteFloat(rect.x);
            WriteFloat(rect.y);
            WriteFloat(rect.width);
            WriteFloat(rect.height);          
        }

        public void WriteRay2D(Ray2D ray)
        {
            WriteVector2(ray.direction);
            WriteVector2(ray.origin);
        }

        public void WriteRay(Ray ray)
        {
            WriteVector3(ray.direction);
            WriteVector3(ray.origin);
        }

        public void WritePlane(Vector3 a, Vector3 b, Vector3 c)
        {
            WriteVector3(a);
            WriteVector3(b);
            WriteVector3(c);
        }

    }

    public partial class IncomingMessage
    {
        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Color ReadColorAlpha()
        {
            return new Color(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Color ReadColor()
        {
            return new Color(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Rect ReadRect()
        {
            return new Rect(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Ray2D ReadRay2D()
        {
            return new Ray2D(ReadVector2(), ReadVector2());
        }

        public Ray ReadRay()
        {            
            return new Ray(ReadVector3(), ReadVector3());
        }

        public Plane ReadPlane()
        {
            return new Plane(ReadVector3(), ReadVector3(), ReadVector3());
        }
    }
}
#endif
