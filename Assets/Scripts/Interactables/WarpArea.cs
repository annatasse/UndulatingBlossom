using UnityEngine;

public class WarpArea : MonoBehaviour {

    // Assign in Inspector
    public WarpArea connectedArea;
    public string warpObjectTag;
    public bool receiverOnly = false;
    //

    private Collider2D receivedObject;

    private void OnTriggerEnter2D(Collider2D other) {
        // A static rigid body doesn't trigger this method, needs to be kinematic or dynamic.
        if (!receiverOnly && other.CompareTag(warpObjectTag)) {
            connectedArea.Receive(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
         if (other == receivedObject) {
            receivedObject = null;
        }
    }

    public void Receive(Collider2D receivedObject) {
        if (CanReceive()) {
            receivedObject.transform.position = transform.position;
            this.receivedObject = receivedObject;
        }
    }

    public bool CanReceive() {
        return receivedObject == null;
    }
}
