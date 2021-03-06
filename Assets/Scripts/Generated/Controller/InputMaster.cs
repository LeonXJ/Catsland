// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Catsland.Scripts.Controller
{
    public class @InputMaster : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputMaster()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""General"",
            ""id"": ""7e8a5233-48af-4854-9bec-097e1f69a2a7"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8b6c093a-cbc5-4423-8085-7eb066c07572"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Value"",
                    ""id"": ""0dca4c2d-80d8-43a7-b3c8-a01ff177a050"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7d847f33-a83a-4977-9e9e-362eb3675038"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""bd72060e-6fac-458a-b16f-21506a508b19"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Focus"",
                    ""type"": ""Value"",
                    ""id"": ""d86840b4-e277-46c1-9769-2b6d0ccb6bf2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""JumpHigher"",
                    ""type"": ""Value"",
                    ""id"": ""d9bd34a0-48fa-4a0b-8086-a73ff3da8924"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""36ed3438-f553-470a-9b94-2a203dabfcb6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SceneTransitConfirm"",
                    ""type"": ""Value"",
                    ""id"": ""8f17002f-ba91-4879-bcdf-fb38fc7a2f07"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Option"",
                    ""type"": ""Button"",
                    ""id"": ""42a7d43a-7ec5-4bc0-97b1-6a6afa3a486e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MenuNext"",
                    ""type"": ""Button"",
                    ""id"": ""60a98a24-01ed-4bcb-b71b-2ddb1c1ed210"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MenuPrevious"",
                    ""type"": ""Button"",
                    ""id"": ""2f5b089c-67b1-445d-a997-a647ae287c24"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""30838a10-3af1-4f4b-af3f-2b0226f754fe"",
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
                    ""id"": ""467843d7-5908-4d25-936f-885d6814baa8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6a52c3f5-b859-4402-bbff-f429e87a79a4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""31104cd8-8b67-4537-8ea8-e80c4ed22c69"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4a0b2ae7-e74c-455d-8e0e-280e0d9e2433"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""cf870307-e02e-4ef3-985d-3bc3e6289d2d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84964706-8a8c-442b-8e92-b58efa1783d8"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a48eb60-be4a-4f8b-87da-4e0e44f798af"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa406d23-e631-40bc-a4e3-a83e9b045bcd"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ade5821b-ebdb-4c3a-969c-a073a52355a5"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69186667-4b53-4e13-b16d-63bb6be2e407"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0240d318-3479-4cb7-b699-df6d21b16b58"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47fc7074-6eeb-4e3e-8529-edd98b6c5cfe"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Focus"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee33d60b-97fa-4c50-b081-2cff31eec74c"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""Focus"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a183f15-9de5-46a4-a602-647f16b91109"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""JumpHigher"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86ded503-a18e-475d-b003-158bcf292a03"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""JumpHigher"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e41ea99-3ff3-4eb1-9316-8924c3c08808"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4533e44-c929-4e60-88be-b3d80af28dcf"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb547006-9a0a-4f17-96cf-04c97b6eee4f"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""SceneTransitConfirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""407d34e1-131c-429b-9b2b-c7a4027a20db"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c150d44e-74f7-4062-a1dd-7fadbd8d13ff"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd63a772-82e4-4792-9659-07b8099a807c"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4;Xbox One"",
                    ""action"": ""MenuNext"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06546cc6-65ee-49d8-98aa-8e6fcb07ce67"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox One;PS4"",
                    ""action"": ""MenuPrevious"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""id"": ""5a2aa1e7-2d4f-47c8-b793-e5eda8d95c06"",
            ""actions"": [
                {
                    ""name"": ""Ripple"",
                    ""type"": ""Button"",
                    ""id"": ""8cf853fe-e6c6-44a1-9184-7a2e08c88a0d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LoadScene1"",
                    ""type"": ""Button"",
                    ""id"": ""a5e64995-6e9e-4f42-9aae-35dc54bed274"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fd77f56d-dcb2-455a-8ef1-2b34b78696f3"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Ripple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c2f03fd-786d-4b48-bb9b-822cc44303f2"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LoadScene1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Xbox One"",
            ""bindingGroup"": ""Xbox One"",
            ""devices"": [
                {
                    ""devicePath"": ""<XboxOneGampadiOS>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""PS4"",
            ""bindingGroup"": ""PS4"",
            ""devices"": [
                {
                    ""devicePath"": ""<DualShockGamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // General
            m_General = asset.FindActionMap("General", throwIfNotFound: true);
            m_General_Move = m_General.FindAction("Move", throwIfNotFound: true);
            m_General_Shoot = m_General.FindAction("Shoot", throwIfNotFound: true);
            m_General_Jump = m_General.FindAction("Jump", throwIfNotFound: true);
            m_General_Dash = m_General.FindAction("Dash", throwIfNotFound: true);
            m_General_Focus = m_General.FindAction("Focus", throwIfNotFound: true);
            m_General_JumpHigher = m_General.FindAction("JumpHigher", throwIfNotFound: true);
            m_General_Interact = m_General.FindAction("Interact", throwIfNotFound: true);
            m_General_SceneTransitConfirm = m_General.FindAction("SceneTransitConfirm", throwIfNotFound: true);
            m_General_Option = m_General.FindAction("Option", throwIfNotFound: true);
            m_General_MenuNext = m_General.FindAction("MenuNext", throwIfNotFound: true);
            m_General_MenuPrevious = m_General.FindAction("MenuPrevious", throwIfNotFound: true);
            // Debug
            m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
            m_Debug_Ripple = m_Debug.FindAction("Ripple", throwIfNotFound: true);
            m_Debug_LoadScene1 = m_Debug.FindAction("LoadScene1", throwIfNotFound: true);
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

        // General
        private readonly InputActionMap m_General;
        private IGeneralActions m_GeneralActionsCallbackInterface;
        private readonly InputAction m_General_Move;
        private readonly InputAction m_General_Shoot;
        private readonly InputAction m_General_Jump;
        private readonly InputAction m_General_Dash;
        private readonly InputAction m_General_Focus;
        private readonly InputAction m_General_JumpHigher;
        private readonly InputAction m_General_Interact;
        private readonly InputAction m_General_SceneTransitConfirm;
        private readonly InputAction m_General_Option;
        private readonly InputAction m_General_MenuNext;
        private readonly InputAction m_General_MenuPrevious;
        public struct GeneralActions
        {
            private @InputMaster m_Wrapper;
            public GeneralActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_General_Move;
            public InputAction @Shoot => m_Wrapper.m_General_Shoot;
            public InputAction @Jump => m_Wrapper.m_General_Jump;
            public InputAction @Dash => m_Wrapper.m_General_Dash;
            public InputAction @Focus => m_Wrapper.m_General_Focus;
            public InputAction @JumpHigher => m_Wrapper.m_General_JumpHigher;
            public InputAction @Interact => m_Wrapper.m_General_Interact;
            public InputAction @SceneTransitConfirm => m_Wrapper.m_General_SceneTransitConfirm;
            public InputAction @Option => m_Wrapper.m_General_Option;
            public InputAction @MenuNext => m_Wrapper.m_General_MenuNext;
            public InputAction @MenuPrevious => m_Wrapper.m_General_MenuPrevious;
            public InputActionMap Get() { return m_Wrapper.m_General; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GeneralActions set) { return set.Get(); }
            public void SetCallbacks(IGeneralActions instance)
            {
                if (m_Wrapper.m_GeneralActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMove;
                    @Shoot.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnShoot;
                    @Shoot.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnShoot;
                    @Shoot.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnShoot;
                    @Jump.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnJump;
                    @Dash.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnDash;
                    @Dash.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnDash;
                    @Dash.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnDash;
                    @Focus.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnFocus;
                    @Focus.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnFocus;
                    @Focus.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnFocus;
                    @JumpHigher.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnJumpHigher;
                    @JumpHigher.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnJumpHigher;
                    @JumpHigher.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnJumpHigher;
                    @Interact.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnInteract;
                    @SceneTransitConfirm.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnSceneTransitConfirm;
                    @SceneTransitConfirm.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnSceneTransitConfirm;
                    @SceneTransitConfirm.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnSceneTransitConfirm;
                    @Option.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnOption;
                    @Option.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnOption;
                    @Option.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnOption;
                    @MenuNext.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMenuNext;
                    @MenuNext.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMenuNext;
                    @MenuNext.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMenuNext;
                    @MenuPrevious.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMenuPrevious;
                    @MenuPrevious.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMenuPrevious;
                    @MenuPrevious.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMenuPrevious;
                }
                m_Wrapper.m_GeneralActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Shoot.started += instance.OnShoot;
                    @Shoot.performed += instance.OnShoot;
                    @Shoot.canceled += instance.OnShoot;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Dash.started += instance.OnDash;
                    @Dash.performed += instance.OnDash;
                    @Dash.canceled += instance.OnDash;
                    @Focus.started += instance.OnFocus;
                    @Focus.performed += instance.OnFocus;
                    @Focus.canceled += instance.OnFocus;
                    @JumpHigher.started += instance.OnJumpHigher;
                    @JumpHigher.performed += instance.OnJumpHigher;
                    @JumpHigher.canceled += instance.OnJumpHigher;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                    @SceneTransitConfirm.started += instance.OnSceneTransitConfirm;
                    @SceneTransitConfirm.performed += instance.OnSceneTransitConfirm;
                    @SceneTransitConfirm.canceled += instance.OnSceneTransitConfirm;
                    @Option.started += instance.OnOption;
                    @Option.performed += instance.OnOption;
                    @Option.canceled += instance.OnOption;
                    @MenuNext.started += instance.OnMenuNext;
                    @MenuNext.performed += instance.OnMenuNext;
                    @MenuNext.canceled += instance.OnMenuNext;
                    @MenuPrevious.started += instance.OnMenuPrevious;
                    @MenuPrevious.performed += instance.OnMenuPrevious;
                    @MenuPrevious.canceled += instance.OnMenuPrevious;
                }
            }
        }
        public GeneralActions @General => new GeneralActions(this);

        // Debug
        private readonly InputActionMap m_Debug;
        private IDebugActions m_DebugActionsCallbackInterface;
        private readonly InputAction m_Debug_Ripple;
        private readonly InputAction m_Debug_LoadScene1;
        public struct DebugActions
        {
            private @InputMaster m_Wrapper;
            public DebugActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
            public InputAction @Ripple => m_Wrapper.m_Debug_Ripple;
            public InputAction @LoadScene1 => m_Wrapper.m_Debug_LoadScene1;
            public InputActionMap Get() { return m_Wrapper.m_Debug; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
            public void SetCallbacks(IDebugActions instance)
            {
                if (m_Wrapper.m_DebugActionsCallbackInterface != null)
                {
                    @Ripple.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnRipple;
                    @Ripple.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnRipple;
                    @Ripple.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnRipple;
                    @LoadScene1.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnLoadScene1;
                    @LoadScene1.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnLoadScene1;
                    @LoadScene1.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnLoadScene1;
                }
                m_Wrapper.m_DebugActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Ripple.started += instance.OnRipple;
                    @Ripple.performed += instance.OnRipple;
                    @Ripple.canceled += instance.OnRipple;
                    @LoadScene1.started += instance.OnLoadScene1;
                    @LoadScene1.performed += instance.OnLoadScene1;
                    @LoadScene1.canceled += instance.OnLoadScene1;
                }
            }
        }
        public DebugActions @Debug => new DebugActions(this);
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
            }
        }
        private int m_XboxOneSchemeIndex = -1;
        public InputControlScheme XboxOneScheme
        {
            get
            {
                if (m_XboxOneSchemeIndex == -1) m_XboxOneSchemeIndex = asset.FindControlSchemeIndex("Xbox One");
                return asset.controlSchemes[m_XboxOneSchemeIndex];
            }
        }
        private int m_PS4SchemeIndex = -1;
        public InputControlScheme PS4Scheme
        {
            get
            {
                if (m_PS4SchemeIndex == -1) m_PS4SchemeIndex = asset.FindControlSchemeIndex("PS4");
                return asset.controlSchemes[m_PS4SchemeIndex];
            }
        }
        public interface IGeneralActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnShoot(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
            void OnFocus(InputAction.CallbackContext context);
            void OnJumpHigher(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnSceneTransitConfirm(InputAction.CallbackContext context);
            void OnOption(InputAction.CallbackContext context);
            void OnMenuNext(InputAction.CallbackContext context);
            void OnMenuPrevious(InputAction.CallbackContext context);
        }
        public interface IDebugActions
        {
            void OnRipple(InputAction.CallbackContext context);
            void OnLoadScene1(InputAction.CallbackContext context);
        }
    }
}
