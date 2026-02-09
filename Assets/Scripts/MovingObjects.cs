using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    [SerializeField] BoxCollider MovingZone;
    [SerializeField] GameObject ObjectWantToMove;
    [SerializeField] float MoveSpeedLR;
    [SerializeField] float MoveSpeedUD;
    [SerializeField] float MoveSpeedBF;
    [SerializeField] bool LeftRight;
    [SerializeField] bool UpDown;
    [SerializeField] bool BackForth;
    enum MovedDirection
    {
        X_Axis,
        Y_Axis,
        Z_Axis,
        XY_Axis,
        XZ_Axis,
        YZ_Axis,
        XYZ_Axis
    }
    [SerializeField] MovedDirection movedDirection;

    float minX, maxX, minY, maxY, minZ, maxZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 center = MovingZone.bounds.center;
        Vector3 size = MovingZone.bounds.size;

        minX = center.x - size.x / 2;
        maxX = center.x + size.x / 2;
        minY = center.y - size.y / 2;
        maxY = center.y + size.y / 2;
        minZ = center.z - size.z / 2;
        maxZ = center.z + size.z / 2;
    }

    // Update is called once per frame
    void Update()
    {
        MovingObject();
    }

    void MovingObject()
    {
        if (movedDirection == MovedDirection.X_Axis
            || movedDirection == MovedDirection.XY_Axis
            || movedDirection == MovedDirection.XY_Axis
            || movedDirection == MovedDirection.XYZ_Axis)
        {
            float newX;
            if (LeftRight)
            {
                newX = ObjectWantToMove.transform.position.x + MoveSpeedLR * Time.deltaTime;
                if (newX > maxX)
                {
                    LeftRight = false;
                }
            }
            else
            {
                newX = ObjectWantToMove.transform.position.x - MoveSpeedLR * Time.deltaTime;
                if (newX < minX)
                {
                    LeftRight = true;
                }
            }
                ObjectWantToMove.transform.position = new Vector3(newX, ObjectWantToMove.transform.position.y, ObjectWantToMove.transform.position.z);
        }

        if(movedDirection == MovedDirection.Y_Axis 
            || movedDirection == MovedDirection.XY_Axis 
            || movedDirection == MovedDirection.YZ_Axis
            || movedDirection == MovedDirection.XYZ_Axis)
        {
            float newY;
            if (UpDown)
            {
                newY = ObjectWantToMove.transform.position.y + MoveSpeedUD * Time.deltaTime;
                if (newY > maxY)
                {
                    UpDown = false;
                }
            }
            else
            {
                newY = ObjectWantToMove.transform.position.y - MoveSpeedUD * Time.deltaTime;
                if (newY < minY)
                {
                    UpDown = true;
                }
            }
            ObjectWantToMove.transform.position = new Vector3(ObjectWantToMove.transform.position.x, newY, ObjectWantToMove.transform.position.z);
        }
        
        if(movedDirection == MovedDirection.Z_Axis 
            || movedDirection == MovedDirection.XZ_Axis
            || movedDirection == MovedDirection.YZ_Axis
            || movedDirection == MovedDirection.XYZ_Axis)
        {
            float newZ;
            if (BackForth)
            {
                newZ = ObjectWantToMove.transform.position.z + MoveSpeedBF * Time.deltaTime;
                if (newZ > maxZ)
                {
                    BackForth = false;
                }
            }
            else
            {
                newZ = ObjectWantToMove.transform.position.z - MoveSpeedBF * Time.deltaTime;
                if (newZ < minZ)
                {
                    BackForth = true;
                }
            }
            ObjectWantToMove.transform.position = new Vector3(ObjectWantToMove.transform.position.x, ObjectWantToMove.transform.position.y, newZ);
        }
    }
}
