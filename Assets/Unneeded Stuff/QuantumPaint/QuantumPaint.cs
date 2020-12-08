using Qiskit;
using QuantumImage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuantumPaint : MonoBehaviour {

    [Header("Important")]
    public ImageSize Size;
    public Color PaintColor = Color.white;

    public ReflectionType Type;
    [Range(0, 1)]
    public float Strength;
    public int AxisToReflect;



    //public Reflection ReflectionToAdd;

    Gate GateToAdd = new Gate();

    [Header("Advanced")]
    public bool ShowAdvanced;


    public bool UseSimpleEncoding = false;
    public bool RenormalizeImage = true;


    public QuantumCircuit Circuit;
    public List<Gate> Gates;
    public RawImage TargetImage;

    [Header("File Management")]
    public string FileName;
    [Tooltip("The folder name (under Assets) in which the file will be stored (when pressing saving file direct).")]
    public string FolderName = "Visuals";

    public int logSize = 2;


    public GameObject[] Vertical;
    public GameObject[] Horizontal;

    Texture2D startTexture;
    Texture2D currentTexture;
    int size;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Test() {
        Debug.Log("Test");
        TargetImage.texture = CircuitToTexture();
    }


    public Texture2D CircuitToTexture() {


        Texture2D outPutTexture;

        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        //QuantumCircuit green;
        //QuantumCircuit blue;


        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(startTexture, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }


        Circuit = red;


        red.Gates = Gates;
        //blue.Gates = Gates;
        //green.Gates = Gates;

        double[,] redData;//,  greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, size, size, RenormalizeImage);
            //greenData = QuantumImageHelper.CircuitToImage(green, size, size);
            //blueData = QuantumImageHelper.CircuitToImage(blue, size, size);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, size, size, RenormalizeImage);
            //greenData = QuantumImageHelper.CircuitToHeight2D(green, size, size);
            //blueData = QuantumImageHelper.CircuitToHeight2D(blue, size, size);
        }

        //outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, redData, redData, PaintColor.r,PaintColor.g, PaintColor.b);
        outPutTexture.filterMode = FilterMode.Point;
        outPutTexture.wrapMode = TextureWrapMode.Clamp;

        return outPutTexture;

    }

    public void InitializeImage() {

        size = 4;

        switch (Size) {
            case ImageSize._4x4:
                size = 4;
                logSize = 2;
                break;
            case ImageSize._8x8:
                logSize = 3;
                size = 8;
                break;
            case ImageSize._16x16:
                logSize = 4;
                size = 16;
                break;
            case ImageSize._32x32:
                logSize = 5;
                size = 32;
                break;
            case ImageSize._64x64:
                logSize = 6;
                size = 64;
                break;
            case ImageSize._128x128:
                logSize = 7;
                size = 128;
                break;
            default:
                break;
        }

        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                texture.SetPixel(i, j, Color.black);
            }
        }

        texture.SetPixel(0, 0, PaintColor);

        texture.Apply();

        startTexture = texture;

        Gates = new List<Gate>();
        TargetImage.texture = startTexture;
        currentTexture = startTexture;
        ShowPreview();
    }

    public void AddGate() {
        GateToAdd = new Gate();
        GateToAdd.CircuitType = CircuitType.RX;
        GateToAdd.Theta = Strength * Mathf.PI;
        GateToAdd.First = AxisToReflect-1;
        if (Type == ReflectionType.Vertical) {
            GateToAdd.First = AxisToReflect-1 + logSize;
        }
        Gates.Add(GateToAdd);
        ApplyGates();
    }


    public void Undo() {
        if (Gates.Count>0) {
            Gates.RemoveAt(Gates.Count-1);
            ApplyGates();
        }
    }

    public void ApplyGates() {
        currentTexture = CircuitToTexture();
        TargetImage.texture = currentTexture;

    }





    public Texture2D CreateBlur(Texture2D inputeTExture, float rotation) {

        Texture2D outPutTexture;


        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(inputeTExture, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(inputeTExture, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(inputeTExture, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }

        //QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.R);
        //QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.G);
        //QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.B);

        //applying the rotation generating the blur effect
        ApplyPartialQ(red, rotation);
        ApplyPartialQ(green, rotation);
        ApplyPartialQ(blue, rotation);

        Gates = red.Gates;
        double[,] redData, greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            greenData = QuantumImageHelper.CircuitToImage(green, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            blueData = QuantumImageHelper.CircuitToImage(blue, inputeTExture.width, inputeTExture.height, RenormalizeImage);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            greenData = QuantumImageHelper.CircuitToHeight2D(green, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            blueData = QuantumImageHelper.CircuitToHeight2D(blue, inputeTExture.width, inputeTExture.height, RenormalizeImage);
        }

        outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        return outPutTexture;

    }

    public Texture2D ApplyGates(Texture2D InputTexture1) {

        Texture2D outPutTexture;


        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }

        //QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.R);
        //QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.G);
        //QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.B);

        red.Gates = Gates;
        blue.Gates = Gates;
        green.Gates = Gates;

        double[,] redData, greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.CircuitToImage(green, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.CircuitToImage(blue, InputTexture1.width, InputTexture1.height);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.CircuitToHeight2D(green, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.CircuitToHeight2D(blue, InputTexture1.width, InputTexture1.height);
        }

        outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        return outPutTexture;
    }

    public void ShowPreview() {
        GameObject[] highlights = Horizontal;
        switch (Type) {
            case ReflectionType.Horizontal:
                highlights = Horizontal;
                break;
            case ReflectionType.Vertical:
                highlights = Vertical;
                break;
            default:
                break;
        }
        int target = logSize - AxisToReflect;
        for (int i = 0; i < Horizontal.Length; i++) {
            Horizontal[i].SetActive(false);
        }
        for (int i = 0; i < Vertical.Length; i++) {
            Vertical[i].SetActive(false);
        }
        if (target<=highlights.Length) {
            highlights[target].SetActive(true);
        }
        

    }


    /// <summary>
    /// Applies a partial rotation (in radian) to each qubit of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public static void ApplyPartialQ(QuantumCircuit circuit, float rotation) {
        for (int i = 0; i < circuit.NumberOfQubits; i++) {
            circuit.RX(i, rotation);
        }
    }

    public void SaveFile() {
        string path = Path.Combine(Application.dataPath, FolderName, FileName + ".png");
        File.WriteAllBytes(path, currentTexture.EncodeToPNG());
    }

    public enum ImageSize {
        _4x4,
        _8x8,
        _16x16,
        _32x32,
        _64x64,
        _128x128
    }

    public enum ReflectionType {
        Horizontal,
        Vertical
    }

    /*
    [System.Serializable]
    public class Reflection {
        public ReflectionType Type;
        [Range(0, 1)]
        public float Strength;
        [Range(0, 1)]
        public int AxisToReflect;
    }
    */
}
