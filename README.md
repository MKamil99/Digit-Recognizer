# Digit Recognizer
**Digit Recognizer** is a project written for Machine Learning course on the IV semester of Informatics on Silesian University of Technology.

### Table of contents
* [Project description](#project-description)
* [How to use main application](#how-to-use-main-application)
* [How to use Learning Place](#how-to-use-learning-place)

### Project description
Project **Digit Recognizer** was made for **recognizing handwritten equations and calculating their results**.
It consists of two programs. Main application is a window with white field in which user can write natural 
numbers and basic arithmetical signs (+, -, *, /). Thanks to previously prepared artificial neural network, 
the program analyzes user's input and displays result of the equation.

Second program is so-called **Learning Place** which is simply **artificial neural network that waits for learning**. 
To teach it we use MNIST database which consists of 70 000 handwritten digits. We also prepared own digits' 
database as well as own arithemtical signs' databse, so the network has become more precise. Network used 
in this process has 784 input neurons (MNIST database's digits are saved in dimensions 28x28 and there needs 
to be one neuron per pixel), 14 output neurons (for classifying; 10 digits and 4 arithmetical signs) and 4 hidden layers
with 100 neurons each.
	
### How to use main application
To run main application, you just need two files: **DigitRecognizer.exe** and **weights.txt**. Second one is **network configuration** 
and consists of thousands of lines. In first line you can find there some numbers; first one is **alpha coefficient used in bipolar 
linear activation function**, other ones are **numbers of neurons on specific layers** (input, hidden, output). Other lines are **weights
of network's neurons**.

<p align="center">
  <img src="https://user-images.githubusercontent.com/43967269/107963517-916d9980-6fa8-11eb-99ec-aa3d0668d30b.png" alt="DigitRecognizer" width="500">
</p>

Before you start writing an equation in white field, you need to **launch the network** by clicking on the first button. 
Then, to **calculate the result** after writing an equation, you need to click the last button. If you want to **clear the field**, 
you need to click the button which is in the middle. And that's it!

### How to use Learning Place
If main application doesn't work fine, **the network needs to learn again**. This time, you need more files: **NeuralNetwork.exe**, **weights.txt** 
and whole **Datasets** directory with MNIST database's files and some .png files. Program uses all signs*.png and digits*.png files, so **you can add your
own digits and arithmetical signs** to make the program even more precise in analyzing your handwriting.

You may also want to **change the build of the network**. In this case you just have to edit code in **Program.cs**. 
From the code that you can see below, simply uncomment line with Network constructor and comment the line 
with LoadNetworkFromFile function. Then change it's arguments - alpha coefficient and table of hidden layers' neurons' amounts.
You can also **edit parameters of Train function**: change amount of iterations, disable showing info 
about current Mean Square Error (change true to false) or enable breaking the loop in situations when error grows 
(add 4th argument: true). If your own handwritten datasets are much bigger you can also **change MNISTDatasetSizeDivider value** to smaller number - this variable helps in keeping good proportions between MNIST datasets and own datasets.

```
int MNISTDatasetSizeDivider = 50; // 1 -> 60000+10000; 5 -> 12000+2000; 10 -> 6000+1000; and so on...
double[][][] datasets = Data.PrepareDatasets(MNISTDatasetSizeDivider);

//Network network = new Network(0.8, datasets[0][0].Length, new int[] { 100, 100, 100, 100}, datasets[1][0].Length);
Network network = Network.LoadNetworkFromFile("weights.txt");
network.CalculatePrecision(datasets);
network.Train(datasets, 15, true);
network.CalculatePrecision(datasets);
```
