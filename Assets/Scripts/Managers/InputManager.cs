using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void OnInputMouseDown(Actions action, Vector2Int gridPos, GameObject go);
    public static event OnInputMouseDown onInputMouseDown;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin,ray.direction,Color.yellow);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                
                if(hit.transform.tag == "tag_enemy") {
                    onInputMouseDown(Actions.ATTACK, hit.transform.gameObject.GetComponent<Unit>().GridPos, hit.transform.gameObject);
                }
                if (hit.transform.tag == "tag_node") {
                    onInputMouseDown(Actions.MOVE, hit.transform.gameObject.GetComponent<Node>()._gridPosition, null);
                }
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            Camera.main.transform.localPosition += Camera.main.transform.forward * Input.GetAxis("Mouse ScrollWheel") * 5;
        }

        if (Input.GetKey(KeyCode.D)) {
            Camera.main.transform.localPosition += Camera.main.transform.right * 0.1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            Camera.main.transform.localPosition -= Camera.main.transform.right * 0.1f;
        }
        if (Input.GetKey(KeyCode.W)) {
            Vector3 cameraMovForward = Camera.main.transform.TransformDirection(Vector3.forward);
            Camera.main.transform.Translate((cameraMovForward.x * 0.1f), 0, (cameraMovForward.z * 0.1f), Space.World);
        }
        if (Input.GetKey(KeyCode.S)) {
            Vector3 cameraMovForward = Camera.main.transform.TransformDirection(Vector3.forward);
            Camera.main.transform.Translate((cameraMovForward.x * -0.1f), 0, (cameraMovForward.z * -0.1f), Space.World);
        }
    }
}