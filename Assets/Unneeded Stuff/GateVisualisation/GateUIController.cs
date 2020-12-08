using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Qiskit;

public class GateUIController : MonoBehaviour {
    public static GateUIController Instance;

    public Texture2D InputTexture;

    //linking
    public SelectableGate GatePrefab;
    public GameObject UI;
    public Dropdown TypeChooser;
    public GameObject RotationHolder;
    public GameObject TargetUI;
    public RawImage Visualisation;

    public Transform Target;
    public ImageCreation ImageCreator;


    public Gate[] operations;
    public SelectableGate[][] allGates;

    SelectableGate selected;
    Gate currentGate=new Gate();

    public float cellSizeX = 1.05f;
    public float cellSizeY = 1.05f;

    public TerrainGenerator Generator;


    public bool PresentationMode = true;

    bool targetMode = false;
    bool nextIsCRX = false;

    public GameObject[] Lines;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
            if (PresentationMode) {
                Test();
            } else {
                int qubits = Mathf.CeilToInt(Mathf.Log(InputTexture.width* InputTexture.height) / Mathf.Log(2));
                Initialize(qubits, 15);
            }
        } else {
            Destroy(this);
        }
    }


    public void Initialize(int numQUbits, int maxgates) {
        if (Target == null) {
            Target = this.transform;
        }

        operations = new Gate[maxgates];
        for (int i = 0; i < maxgates; i++) {
            operations[i] = new Gate();
            operations[i].CircuitType = CircuitType.M;
        }

        allGates = new SelectableGate[maxgates][];
        for (int i = 0; i < maxgates; i++) {
            allGates[i] = new SelectableGate[numQUbits];
        }

        for (int i = 0; i < Lines.Length; i++) {
            if (i>=numQUbits) {
                Lines[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < maxgates; i++) {
            for (int j = 0; j < numQUbits; j++) {
                //allGates[i][j] = Instantiate<SelectableGate>(GatePrefab, Target.position + new Vector3(i * cellSizeX, -j * cellSizeY, 0), Quaternion.identity, this.transform);
                allGates[i][j] = Instantiate<SelectableGate>(GatePrefab, Target.position, Quaternion.identity, this.transform);
                allGates[i][j].transform.localPosition += new Vector3(i * cellSizeX, -j * cellSizeY, 0);
                //allGates[i][j].transform.localRotation = Quaternion.Euler(0, 180, 0);
                allGates[i][j].Initialise(i, j);
                allGates[i][j].Reset();
                //allGates[i][j].ActivateVisuals();
            }
        }

        HideUI();

        Invoke(nameof(ApplyGates), 0.1f);

    }

    public void Test() {

        int numQUbits = 10;
        int maxgates = 10;

        if (Target == null) {
            Target = this.transform;
        }

        operations = new Gate[maxgates];
        for (int i = 0; i < maxgates; i++) {
            operations[i] = new Gate();
            operations[i].CircuitType = CircuitType.M;
        }

        allGates = new SelectableGate[maxgates][];
        for (int i = 0; i < maxgates; i++) {
            allGates[i] = new SelectableGate[numQUbits];
        }

        for (int i = 0; i < maxgates; i++) {
            for (int j = 0; j < numQUbits; j++) {
                //allGates[i][j] = Instantiate<SelectableGate>(GatePrefab, Target.position + new Vector3(i * cellSizeX, -j * cellSizeY, 0), Quaternion.identity, this.transform);
                allGates[i][j] = Instantiate<SelectableGate>(GatePrefab, Target.position, Quaternion.identity, this.transform);
                allGates[i][j].transform.localPosition += new Vector3(i * cellSizeX, -j * cellSizeY, 0);
                //allGates[i][j].transform.localRotation = Quaternion.Euler(0, 180, 0);
                allGates[i][j].Initialise(i, j);
                allGates[i][j].Reset();
                //allGates[i][j].ActivateVisuals();
            }
        }

        for (int i = 0; i < 5; i++) {
            Select(allGates[i][i]);
            if (i%2==0) {
                SelectRX();
            } else if (i==3 || i ==5) {
                SelectH();
            } else {
                SelectX();
            }
        }
        Generator?.gameObject.SetActive(true);
        ApplyGates();
        Generator?.gameObject.SetActive(false);
    }

    public void GenerateTerrain() {
        Generator.InputTexture = ImageCreator.OutputTexture;
        Generator.Generate2DData();
        Generator.Generate3DDataFrom2DData();
        Generator.GenerateMesh();
    }

    public void Clear() {
        int numQUbits = 10;
        int maxgates = 10;

        for (int i = 0; i < maxgates; i++) {
            for (int j = 0; j < numQUbits; j++) {
                DestroyImmediate(allGates[i][j].gameObject);
            }
        }

    }


    public void Select(SelectableGate gate) {
        if (!targetMode) {
            selected = gate;
            selected.ActivateVisuals();
            currentGate.First = selected.PosY;
            ShowUI();
        } else {
            targetMode = false;
            allGates[selected.PosX][gate.PosY].SetName("T");
            allGates[selected.PosX][gate.PosY].SetRotation("");
            allGates[selected.PosX][gate.PosY].ActivateVisuals();
            currentGate.Second = gate.PosY;
            if (nextIsCRX) {
                gate.ActivateRX();
            } else {
                gate.ActivateX();
            }
            Apply();
        }
    }


    public void ChooseRotation(string rotation) {
        if (rotation.Length==0) {
            currentGate.Theta = 0;
        }
        int rot = int.Parse(rotation);
        currentGate.Theta = rot;
    }

    public void ChooseType(int chosenType) {
        RotationHolder.SetActive(false);
        switch (chosenType) {
            case 0:
                currentGate.CircuitType = CircuitType.X;
                selected.ActivateX();
                break;
            case 1:
                currentGate.CircuitType = CircuitType.RX;
                RotationHolder.SetActive(true);
                selected.ActivateRX();
                break;
            case 2:
                currentGate.CircuitType = CircuitType.H;
                selected.ActivateH();
                break;
            case 3:
                currentGate.CircuitType = CircuitType.CX;
                selected.ActivateDot();
                targetMode = true;
                break;
            case 4:
                currentGate.CircuitType = CircuitType.CRX;
                targetMode = true;
                selected.ActivateDot();
                nextIsCRX = true;
                RotationHolder.SetActive(true);
                break;
            case 5:
                currentGate.CircuitType = CircuitType.X;
                selected.ActivateVisuals();
                break;
            default:
                break;
        }
    }

    public void Close() {
        selected = null;
        HideUI();
    }

    public void Apply() {
        if (targetMode) {
            HideUI();
            return;
        }

        operations[selected.PosX].CircuitType = currentGate.CircuitType;
        operations[selected.PosX].Theta = currentGate.Theta*2*Mathf.PI/360;
        operations[selected.PosX].First = currentGate.First;
        operations[selected.PosX].Second = currentGate.Second;

        selected.SetName(currentGate.CircuitType.ToString());
        if (RotationHolder.activeSelf && currentGate.Theta>Mathf.Epsilon) {
            selected.SetRotation(Mathf.RoundToInt((float)currentGate.Theta).ToString());
        } else {
            selected.SetRotation("");
        }
        HideUI();
        Invoke(nameof(ApplyGates), 0.1f);
    }

    public void ApplyGates() {
        ImageCreator.InputTexture1 = InputTexture;
        List<Gate> gateList = new List<Gate>();
        for (int i = 0; i < operations.Length; i++) {
            if (operations[i].CircuitType!= CircuitType.M) {
                gateList.Add(operations[i]);
            }
        }
        ImageCreator.BlurCircuit = gateList;
        ImageCreator.ApplyGates();
        Visualisation.texture = ImageCreator.OutputTexture;


        if (Generator!=null && Generator.gameObject.activeSelf) {
            Debug.Log("Applied Gates " + Generator.gameObject.activeSelf);
            GenerateTerrain();
        }
    }


    public void Delete() {
        Reset(selected.PosX);
        HideUI();
        targetMode = false;
        TargetUI.SetActive(false);
        Invoke(nameof(ApplyGates), 0.1f);

    }

    public void ShowUI() {
        int number = selected.PosX;
        int selectedPos = 0;
        Debug.Log(operations[number].CircuitType);
        switch (operations[number].CircuitType) {
            case CircuitType.X:
                break;
            case CircuitType.RX:
                selectedPos = 1;
                break;
            case CircuitType.H:
                selectedPos = 2;
                break;
            case CircuitType.CX:
                selectedPos = 3;
                break;
            case CircuitType.CRX:
                selectedPos = 4;
                break;
            case CircuitType.M:
                selectedPos = 5;
                break;
            default:
                break;
        }
        if (selectedPos<5) {
            TypeChooser.value = selectedPos;
        }
        ChooseType(selectedPos);
        UI.SetActive(true);

    }

    public void HideUI() {
        UI.SetActive(false);
    }


    public void Reset(int row) {
        for (int i = 0; i < allGates[row].Length; i++) {
            allGates[row][i].Reset();
        }
        operations[row].CircuitType = CircuitType.M;
    }

    public void SetRowActive(int row, bool active) {
        for (int i = 0; i < allGates[row].Length; i++) {
            allGates[row][i].SetActive(active);
        }
    }


    public void SelectX() {
        ChooseType(0);
        Apply();
    }

    public void SelectRX() {
        ChooseRotation("90");
        ChooseType(1);
        Apply();
    }

    public void SelectH() {
        ChooseType(2);
        Apply();
    }

    public void SelectCX() {
        ChooseType(3);
        Apply();
    }



}
