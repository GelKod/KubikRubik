using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 localRotation = new Vector3(-90, 0, 0);
    Vector3 mouseStartPos;
    Vector3 mouseEndPos;
    bool cameraDisabled = false,
        rotateDisabled = false;
    public CubeManager CubeMan;
    List<GameObject> pieces = new List<GameObject>(),
        planes = new List<GameObject>();

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (!rotateDisabled)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    cameraDisabled = true;

                    if (pieces.Count < 2 &&
                        !pieces.Exists(x => x == hit.collider.transform.parent.gameObject) &&
                        hit.transform.parent.gameObject != CubeMan.gameObject)
                    {
                        pieces.Add(hit.collider.transform.parent.gameObject);
                        planes.Add(hit.collider.gameObject);
                    }
                    else if (pieces.Count == 2)
                    {
                        mouseEndPos = Input.mousePosition;
                        Vector3 mouseDirection = mouseEndPos - mouseStartPos;

                        // Передаем направление движения мыши
                        CubeMan.DetectRotate(pieces, planes, mouseDirection);
                        rotateDisabled = true;
                    }
                }
            }
            if (!cameraDisabled)
            {
                rotateDisabled = true;
                localRotation.x += Input.GetAxis("Mouse X") * 15;
                localRotation.y += Input.GetAxis("Mouse Y") * -15;
                localRotation.y = Mathf.Clamp(localRotation.y, -90, 90);
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            pieces.Clear();
            planes.Clear();
            cameraDisabled = rotateDisabled = false;
        }

        Quaternion qt = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, qt, Time.deltaTime * 15);
    }


}
