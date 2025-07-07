using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour {

    public void Close(InputAction.CallbackContext context) {
        if (context.performed && MenuManager.instance.currentMenu == MenuManager.Menu.Pause) {
            MenuManager.instance.CloseMenu();
        }
    }

    public void Cancel(InputAction.CallbackContext context) {
        if (MenuManager.instance.currentMenu == MenuManager.Menu.CharacterSelection) {
            return;
        }
        if (context.performed) {
            MenuManager.instance.PreviousMenu();
        }
    }
}
