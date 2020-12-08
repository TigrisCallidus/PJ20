using Qiskit.Float;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTest : MonoBehaviour
{
    public int Times = 100000;

    public int Repeat = 100;

    // Start is called before the first frame update
    void Start()
    {
        prepareTest();
        Application.targetFrameRate = 30;
        for (int i = 0; i < Repeat; i++) {

            Debug.Log(CopyTest4());
            Debug.Log(CopyTest5());

            //Debug.Log(IncTest());
            //Debug.Log(AddTest());
            //Debug.Log(MultTest());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public int IncTest() {
        int result = 0;
        for (int i = 0; i < Times; i++) {
            result++;
        }
        return result;
    }

    public int AddTest() {
        int result = 0;
        for (int i = 0; i < Times; i++) {
            result+=3;
        }
        return result;
    }

    public int MultTest() {
        int result = 1;
        for (int i = 0; i < Times; i++) {
            result *= 3;
        }
        return result;
    }

    public int loop1Test() {
        int result = 0;
        int value = 5;
        for (int i = 0; i < Times; i++) {
            for (int j = 0; j < 3; j++) {
                int count = j;
                for (int k = 0; k < 3; k++) {
                    result = count;
                    count += value;
                }
            }
        }
        return 0;
    }


    public int loop2Test() {
        int result = 0;
        int value = 5;
        for (int i = 0; i < Times; i++) {
            for (int j = 0; j < 3; j++) {
                int count = j+3*value;
                for (int k = j; k < count; k+=value) {
                    result = count;
                }
            }
        }
        return 0;
    }
    ComplexNumberFloat[] Test;

    void prepareTest() {
        Test = new ComplexNumberFloat[Times];
        for (int i = 0; i < Test.Length; i++) {
            Test[i].Real = i;
            Test[i].Complex = -i;
        }
    }

    public float CopyTest1() {
        ComplexNumberFloat result = new ComplexNumberFloat();
        float parameter1 = 0.25f;
        float parameter2 = 0.75f;
        int half = Times / 2;

        for (int i = 0; i < half; i++) {
            ComplexNumberFloat temp = Test[i];
            int pos2 = i + half;
            ComplexNumberFloat temp2 = Test[pos2];

            result.Real = temp.Real * parameter1 + temp2.Complex * parameter2;
            result.Complex = temp2.Real * parameter2 + temp.Complex * parameter1;
            result.Real = temp2.Real * parameter1 + temp.Complex * parameter2;
            result.Complex = temp.Real * parameter2 + temp2.Complex * parameter1;
        }
        return result.Complex + result.Real;
    }

    public float CopyTest2() {
        ComplexNumberFloat result = new ComplexNumberFloat();
        float parameter1 = 0.25f;
        float parameter2 = 0.75f;
        int half = Times / 2;

        for (int i = 0; i < half; i++) {
            int pos2 = i + half;

            result.Real = Test[i].Real * parameter1 + Test[pos2].Complex * parameter2;
            result.Complex = Test[pos2].Real * parameter2 + Test[i].Complex * parameter1;
            result.Real = Test[pos2].Real * parameter1 + Test[i].Complex * parameter2;
            result.Complex = Test[i].Real * parameter2 + Test[pos2].Complex * parameter1;
        }
        return result.Complex + result.Real;
    }


    public float CopyTest3() {
        ComplexNumberFloat result = new ComplexNumberFloat();
        float parameter1 = 0.25f;
        float parameter2 = 0.75f;
        int half = Times / 2;

        for (int i = 0; i < half; i++) {
            int pos2 = i + half;
            float real1 = Test[i].Real;
            float real2 = Test[pos2].Real;
            float complex1 = Test[i].Complex;
            float complex2 = Test[pos2].Complex;

            result.Real = real1 * parameter1 + complex2 * parameter2;
            result.Complex = real2 * parameter2 + complex1 * parameter1;
            result.Real = real2 * parameter1 + complex1 * parameter2;
            result.Complex = real1 * parameter2 + complex2 * parameter1;
        }
        return result.Complex + result.Real;
    }

    public float CopyTest4() {
        ComplexNumberFloat result = new ComplexNumberFloat();
        float parameter1 = 0.25f;
        float parameter2 = 0.75f;
        int half = Times / 2;

        for (int i = 0; i < half; i++) {
            ComplexNumberFloat temp = Test[i];
            int pos2 = i + half;
            ComplexNumberFloat temp2 = Test[pos2];

            Test[i].Real = temp.Real * parameter1 + temp2.Complex * parameter2;
            Test[i].Complex = temp2.Real * parameter2 + temp.Complex * parameter1;
            Test[pos2].Real = temp2.Real * parameter1 + temp.Complex * parameter2;
            Test[pos2].Complex = temp.Real * parameter2 + temp2.Complex * parameter1;
        }
        return result.Complex + result.Real;
    }

    public float CopyTest5() {
        ComplexNumberFloat result = new ComplexNumberFloat();
        float parameter1 = 0.25f;
        float parameter2 = 0.75f;
        int half = Times / 2;

        ComplexNumberFloat temp3;
        ComplexNumberFloat temp4;

        for (int i = 0; i < half; i++) {
            ComplexNumberFloat temp = Test[i];
            int pos2 = i + half;
            ComplexNumberFloat temp2 = Test[pos2];

            temp3.Real = temp.Real * parameter1 + temp2.Complex * parameter2;
            temp3.Complex = temp2.Real * parameter2 + temp.Complex * parameter1;
            temp4.Real = temp2.Real * parameter1 + temp.Complex * parameter2;
            temp4.Complex = temp.Real * parameter2 + temp2.Complex * parameter1;

            Test[i] = temp3;
            Test[pos2] = temp4;
        }
        return result.Complex + result.Real;
    }

}
