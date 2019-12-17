// GENERATED AUTOMATICALLY FROM 'Assets/GlitchEscape/Config/Input/Input.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Input : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @Input()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""866dafb1-08d6-4562-8e7e-0f3bc1f50ad5"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""be6a26df-7131-41ea-a107-b1bcd641d907"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveCameraMouse"",
                    ""type"": ""Value"",
                    ""id"": ""700517d5-7153-49a6-b76d-bdb1528b5bd5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveCameraGamepad"",
                    ""type"": ""Value"",
                    ""id"": ""d42fff3c-727c-402b-ba81-169f0d020367"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomMouse"",
                    ""type"": ""Value"",
                    ""id"": ""9bff5e8c-875c-4478-ab9a-253daac1a65c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomCameraGamepad"",
                    ""type"": ""Value"",
                    ""id"": ""a5cd1944-a3db-421d-bd9d-62195d39e910"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""0b49e7ee-7534-4c42-a276-ee4a988072af"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""e22d947b-de48-4d67-91c4-1d3662754e90"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""bb4c6854-6d6c-4ecb-8ed9-f60dbd6d9f0f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""OpenMenu"",
                    ""type"": ""Button"",
                    ""id"": ""e01be13f-c0ac-4206-bf55-88a7ad6ae1f3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6d3c603e-dc59-4ddb-9ed0-d8881503af3e"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""ad640a21-113e-4812-8818-0a51d05f6625"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9bef1f02-8745-483a-bcb5-5d6284b954af"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5a50c850-8293-41f9-ac73-56f6e336a857"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""35c8d5a7-e180-42c1-acea-69d6951d3f59"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4a64f044-8c0b-4df0-8217-3d142b2f05e8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""6902272f-0264-4a52-9174-139b84496c10"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6cc1eeca-9ffb-4d7e-8783-56d95ef7bf91"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d3bcefd1-b85e-4fbe-8241-c1fd30230ced"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ef069523-f431-4bf1-ac88-b0d526033215"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""019b1a71-601d-4f99-bd7a-8849ec6b99dd"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6b720b34-d0f1-4e87-be1e-eb82399cddc8"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c35b2c29-a85c-4c25-9ab5-afb44672dfa5"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": """",
                    ""action"": ""MoveCameraMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c6b3c90-2a6e-4d6f-a254-601f7102c2c8"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""4ac9674a-4e47-4d82-b46a-09b6ad786ce6"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomCameraGamepad"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""318a11fa-c937-442b-a77c-9fdcdf386009"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomCameraGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""8a6b952a-7ffd-42cb-a48e-6344e5c55b86"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomCameraGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""256b973e-4396-4fbb-8d35-35047152aec1"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62521354-af33-4887-bc62-b654e507c7a8"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""313f8255-1479-4464-aa9a-1154505c81e6"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""939657ea-0b80-4e39-adec-6fa5c793661f"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5169467-31bf-4f85-920a-4a59a095078c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50529083-e943-457d-a59e-cf46709b0ead"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2344a087-8723-4239-9fe1-095caa47502d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0604ee4-893d-4181-9d56-59dfcf7bdc6d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9982a77c-b84c-424c-9f3b-431119d5ecf1"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e94ddae-abcb-48cb-9e8b-e9fd0825cf0f"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5006ce44-b0a3-478a-8843-ea3ede40b02b"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCameraGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""8c543c2c-ab05-4ca0-ae5c-3514d0f8f694"",
            ""actions"": [
                {
                    ""name"": ""MenuAccept"",
                    ""type"": ""Button"",
                    ""id"": ""d8ac58b5-6694-4739-9de1-d86dec12da91"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MenuCancel"",
                    ""type"": ""Button"",
                    ""id"": ""1599e749-e29d-4325-b4a3-3e8c26658f34"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MenuUp"",
                    ""type"": ""Button"",
                    ""id"": ""6b787571-65f9-4219-94a1-aae928aef354"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MenuDown"",
                    ""type"": ""Button"",
                    ""id"": ""4e7c836b-7bd0-460d-9a19-8be9957a23ae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8b68bd8b-f3b1-4ea6-9046-874b19a441f1"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuAccept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19ae8d18-4394-47f6-995f-6d012746e7f8"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuAccept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86b23b68-efcf-42c3-b4fc-811a0f271f98"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuAccept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5cd57751-b2b4-48a3-aac3-6ad5f33525a2"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuCancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39f8cab9-db9b-4978-8561-fc6c93163f6d"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuCancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16db34e0-15cd-4a2f-a7e1-83b135891b63"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9fe1bdd1-1341-4423-afa5-b7fdb464003d"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23c0d211-e468-47a0-8e1e-5d5609185ba5"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""692e1eb0-9ce5-4ccc-9e71-4fdfecee3fb1"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""250c4247-3ec2-4c32-8c50-9d583f9e6116"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa770be2-948c-499c-9a06-6ee5e08bb7df"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ffd80acb-dc1f-4c22-9638-521a402f8f2a"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe7ea1d4-41f3-4b2a-83d4-07bdc688ed04"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""id"": ""57dd3258-8978-4c03-8de9-281b9f286267"",
            ""actions"": [
                {
                    ""name"": ""ResetCamera"",
                    ""type"": ""Button"",
                    ""id"": ""c03e7679-4192-4a4b-a16f-8bafaac701c6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ResetPosition"",
                    ""type"": ""Button"",
                    ""id"": ""3d06411a-d3e2-4096-9b35-ffdbb29ee5c2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ddcf26e3-52da-4f79-a690-c95f4bb18899"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d90f97da-1e61-421c-94fa-6c4a68ee64ec"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_MoveCameraMouse = m_Gameplay.FindAction("MoveCameraMouse", throwIfNotFound: true);
        m_Gameplay_MoveCameraGamepad = m_Gameplay.FindAction("MoveCameraGamepad", throwIfNotFound: true);
        m_Gameplay_ZoomMouse = m_Gameplay.FindAction("ZoomMouse", throwIfNotFound: true);
        m_Gameplay_ZoomCameraGamepad = m_Gameplay.FindAction("ZoomCameraGamepad", throwIfNotFound: true);
        m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
        m_Gameplay_Dodge = m_Gameplay.FindAction("Dodge", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_OpenMenu = m_Gameplay.FindAction("OpenMenu", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_MenuAccept = m_UI.FindAction("MenuAccept", throwIfNotFound: true);
        m_UI_MenuCancel = m_UI.FindAction("MenuCancel", throwIfNotFound: true);
        m_UI_MenuUp = m_UI.FindAction("MenuUp", throwIfNotFound: true);
        m_UI_MenuDown = m_UI.FindAction("MenuDown", throwIfNotFound: true);
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_ResetCamera = m_Debug.FindAction("ResetCamera", throwIfNotFound: true);
        m_Debug_ResetPosition = m_Debug.FindAction("ResetPosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_MoveCameraMouse;
    private readonly InputAction m_Gameplay_MoveCameraGamepad;
    private readonly InputAction m_Gameplay_ZoomMouse;
    private readonly InputAction m_Gameplay_ZoomCameraGamepad;
    private readonly InputAction m_Gameplay_Interact;
    private readonly InputAction m_Gameplay_Dodge;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_OpenMenu;
    public struct GameplayActions
    {
        private @Input m_Wrapper;
        public GameplayActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @MoveCameraMouse => m_Wrapper.m_Gameplay_MoveCameraMouse;
        public InputAction @MoveCameraGamepad => m_Wrapper.m_Gameplay_MoveCameraGamepad;
        public InputAction @ZoomMouse => m_Wrapper.m_Gameplay_ZoomMouse;
        public InputAction @ZoomCameraGamepad => m_Wrapper.m_Gameplay_ZoomCameraGamepad;
        public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
        public InputAction @Dodge => m_Wrapper.m_Gameplay_Dodge;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @OpenMenu => m_Wrapper.m_Gameplay_OpenMenu;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @MoveCameraMouse.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCameraMouse;
                @MoveCameraMouse.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCameraMouse;
                @MoveCameraMouse.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCameraMouse;
                @MoveCameraGamepad.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCameraGamepad;
                @MoveCameraGamepad.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCameraGamepad;
                @MoveCameraGamepad.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCameraGamepad;
                @ZoomMouse.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnZoomMouse;
                @ZoomMouse.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnZoomMouse;
                @ZoomMouse.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnZoomMouse;
                @ZoomCameraGamepad.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnZoomCameraGamepad;
                @ZoomCameraGamepad.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnZoomCameraGamepad;
                @ZoomCameraGamepad.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnZoomCameraGamepad;
                @Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Dodge.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDodge;
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @OpenMenu.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnOpenMenu;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @MoveCameraMouse.started += instance.OnMoveCameraMouse;
                @MoveCameraMouse.performed += instance.OnMoveCameraMouse;
                @MoveCameraMouse.canceled += instance.OnMoveCameraMouse;
                @MoveCameraGamepad.started += instance.OnMoveCameraGamepad;
                @MoveCameraGamepad.performed += instance.OnMoveCameraGamepad;
                @MoveCameraGamepad.canceled += instance.OnMoveCameraGamepad;
                @ZoomMouse.started += instance.OnZoomMouse;
                @ZoomMouse.performed += instance.OnZoomMouse;
                @ZoomMouse.canceled += instance.OnZoomMouse;
                @ZoomCameraGamepad.started += instance.OnZoomCameraGamepad;
                @ZoomCameraGamepad.performed += instance.OnZoomCameraGamepad;
                @ZoomCameraGamepad.canceled += instance.OnZoomCameraGamepad;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @OpenMenu.started += instance.OnOpenMenu;
                @OpenMenu.performed += instance.OnOpenMenu;
                @OpenMenu.canceled += instance.OnOpenMenu;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_MenuAccept;
    private readonly InputAction m_UI_MenuCancel;
    private readonly InputAction m_UI_MenuUp;
    private readonly InputAction m_UI_MenuDown;
    public struct UIActions
    {
        private @Input m_Wrapper;
        public UIActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @MenuAccept => m_Wrapper.m_UI_MenuAccept;
        public InputAction @MenuCancel => m_Wrapper.m_UI_MenuCancel;
        public InputAction @MenuUp => m_Wrapper.m_UI_MenuUp;
        public InputAction @MenuDown => m_Wrapper.m_UI_MenuDown;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @MenuAccept.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuAccept;
                @MenuAccept.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuAccept;
                @MenuAccept.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuAccept;
                @MenuCancel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuCancel;
                @MenuCancel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuCancel;
                @MenuCancel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuCancel;
                @MenuUp.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuUp;
                @MenuUp.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuUp;
                @MenuUp.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuUp;
                @MenuDown.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuDown;
                @MenuDown.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuDown;
                @MenuDown.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMenuDown;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MenuAccept.started += instance.OnMenuAccept;
                @MenuAccept.performed += instance.OnMenuAccept;
                @MenuAccept.canceled += instance.OnMenuAccept;
                @MenuCancel.started += instance.OnMenuCancel;
                @MenuCancel.performed += instance.OnMenuCancel;
                @MenuCancel.canceled += instance.OnMenuCancel;
                @MenuUp.started += instance.OnMenuUp;
                @MenuUp.performed += instance.OnMenuUp;
                @MenuUp.canceled += instance.OnMenuUp;
                @MenuDown.started += instance.OnMenuDown;
                @MenuDown.performed += instance.OnMenuDown;
                @MenuDown.canceled += instance.OnMenuDown;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_ResetCamera;
    private readonly InputAction m_Debug_ResetPosition;
    public struct DebugActions
    {
        private @Input m_Wrapper;
        public DebugActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @ResetCamera => m_Wrapper.m_Debug_ResetCamera;
        public InputAction @ResetPosition => m_Wrapper.m_Debug_ResetPosition;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                @ResetCamera.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnResetCamera;
                @ResetCamera.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnResetCamera;
                @ResetCamera.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnResetCamera;
                @ResetPosition.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnResetPosition;
                @ResetPosition.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnResetPosition;
                @ResetPosition.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnResetPosition;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ResetCamera.started += instance.OnResetCamera;
                @ResetCamera.performed += instance.OnResetCamera;
                @ResetCamera.canceled += instance.OnResetCamera;
                @ResetPosition.started += instance.OnResetPosition;
                @ResetPosition.performed += instance.OnResetPosition;
                @ResetPosition.canceled += instance.OnResetPosition;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnMoveCameraMouse(InputAction.CallbackContext context);
        void OnMoveCameraGamepad(InputAction.CallbackContext context);
        void OnZoomMouse(InputAction.CallbackContext context);
        void OnZoomCameraGamepad(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnOpenMenu(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnMenuAccept(InputAction.CallbackContext context);
        void OnMenuCancel(InputAction.CallbackContext context);
        void OnMenuUp(InputAction.CallbackContext context);
        void OnMenuDown(InputAction.CallbackContext context);
    }
    public interface IDebugActions
    {
        void OnResetCamera(InputAction.CallbackContext context);
        void OnResetPosition(InputAction.CallbackContext context);
    }
}
