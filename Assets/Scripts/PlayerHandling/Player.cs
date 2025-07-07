using UnityEngine.InputSystem;

public class Player {

    public PlayerInput playerInput;
    public CharacterName character;
    public ControllerType controllerType;
    public bool infiniteDash;
    public bool invincible;

    public Player(PlayerInput playerInput, CharacterName character) {
        this.playerInput = playerInput;
        this.character = character;
        controllerType = GetInputDevice(playerInput);
        infiniteDash = false;
        invincible = false;
    }

    public Player(CharacterName character) {
        this.character = character;
        playerInput = null;
        controllerType = ControllerType.Other;
        infiniteDash = false;
        invincible = false;
    }

    public void SetPlayerInput(PlayerInput playerInput) {
        this.playerInput = playerInput;
        controllerType = GetInputDevice(playerInput);
    }

    public void RemovePlayerInput() {
        playerInput = null;
        controllerType = ControllerType.Other;
    }

    // I wasn't able to test this with controllers yet. It works for Keyboard.
    private ControllerType GetInputDevice(PlayerInput playerInput) {
        // Debug.Log("--------- Get Controller Type ---------");
        foreach (InputDevice device in playerInput.devices) {
            // Debug.Log("----");
            // Debug.Log("Display Name: " + device.displayName);
            // Debug.Log("Name: " + device.name);
            if (device is Keyboard) {
                // Debug.Log($"Device: Keyboard");
                // Debug.Log("Found Keyboard");
                // Debug.Log("--------------------------------------------");
                return ControllerType.Keyboard;
            } else if (device is Gamepad gamepad) {
                // Debug.Log($"Device: Gamepad");
                // Debug.Log($" - Product: {gamepad.description.product}");
                // Debug.Log($" - Manufacturer: {gamepad.description.manufacturer}");
                // Debug.Log($" - Interface Name: {gamepad.description.interfaceName}");
                // Debug.Log($" - Device Class: {gamepad.description.deviceClass}");

                if (CheckForXBox(device)) {
                    // Debug.Log("Found XBox Controller");
                    // Debug.Log("--------------------------------------------");
                    return ControllerType.XBox;
                }

                if (CheckForPlayStaition(device)) {
                    // Debug.Log("Found PlayStation Controller");
                    // Debug.Log("--------------------------------------------");
                    return ControllerType.PlayStation;
                }

                // Debug.Log("Gamepad did not match!");
            } else {
                // Debug.Log("Device:" + device.GetType());
            }
            // Debug.Log("----");
        }
        // Debug.Log("Not identified, returning " + ControllerType.Other);
        return ControllerType.Other;
    }

    private bool CheckForXBox(InputDevice device) {
        return
            device.displayName.ToLower().Contains("xbox") ||
            device.description.product.ToLower().Contains("xbox") ||
            device.description.manufacturer.ToLower().Contains("microsoft");
    }

    private bool CheckForPlayStaition(InputDevice device) {
        return
            device.displayName.ToLower().Contains("ps") ||
            device.description.product.ToLower().Contains("ps") ||
            device.description.manufacturer.ToLower().Contains("sony");
    }
}
