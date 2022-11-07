app (string[] args, entry, name):
	name = "TestApp"
	entry = Main
	args = ["testArg0", "testArg1", "testArg2"]

	void Main (string[] args):
		print: "Hello World!"
		for (int i, i < 3, i++):
			print: CreateIndexMessage(i)
		
		//expected output: 
		//Hello World!
		//i=0
		//i=1
		//i=2
	
		TestClass testClass
	
	

	string CreateIndexMessage(int index):
		return: "i=" + index
		

	template TestClass:
		string name
		int age
		string address
		int height
	
		void PrintInfo():
			print: "Name: " + name
			print: "Age: " + age
			print: "Address: " + address
			print: "Height: " + height
			print: ToString()
		
		string ToString():
			return: "Name: " + name + ", Age: " + age + ", Address: " + address + ", Height: " + height
		
		//expected output:
		//Name: John
		//Age: 25
		//Address: 123 Main St.
		//Height: 65
	



app TestApp(string[] args):
	void Main:
		print: "Hello World!"
	
	class TestClass:
		string name = "John"
		int age = 25
		string address = "123 Main St."
		int height = 65
		
		void PrintInfo():
			print: "Name: " + name
			print: "Age: " + age
			print: "Address: " + address
			print: "Height: " + height
			print: ToString()
			
		string ToString():
			return: "Name: " + name + ", Age: " + age + ", Address: " + address + ", Height: " + height
	