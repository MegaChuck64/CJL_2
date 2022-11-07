#if DEBUG
using CJL;

args = new string[]
{
    "ScopeTest.cj"
};


#endif

var path = args[0];

var input = File.ReadAllText(path);
var tokens = Tokenizer.Tokenize(input);

Console.Read();

//var lines = File.ReadAllLines(path);
//var tokens = new List<Token>();

//int lineCount = 0;
//int column = 0;
//foreach (var line in lines)
//{
//    foreach (var chr in line)
//    {
//        var token = new Token
//        {
//            Value = chr,
//            Line = lineCount,
//            Column = column++
//        };
//        tokens.Add(token);
//    }
    
//    tokens.Add(new Token
//    {
//        Value = ' ',
//        Line = lineCount,
//        Column = column
//    });

//    column = 0;
//    lineCount++;
//}

//List<Word> words = new();
//foreach (var token in tokens.GroupBy(y=>y.Line))
//{
//    var word = new Word(); 
//    foreach (var f in token)
//    {
//        if (char.IsWhiteSpace(f.Value))
//        {
//            if (word.Tokens.Count > 0)
//                words.Add(word);
            
//            word = new Word();
//            continue;
//        }        
//        else 
//        {
//            word.Tokens.Add(f);
//        }
//    }
//}

//foreach (var word in words.GroupBy(y => y.Tokens.FirstOrDefault().Line))
//{
//    foreach (var f in word)
//    {
//        Console.Write(f.ToString());
//    }
//    Console.WriteLine();
//}

////scope

////app => scope
////entry => scope
////classes => scope
////function => scope
////statement => scope
////expression => scope

////int i = 20
////int i


//struct Token
//{
//    public char Value;
//    public int Line;
//    public int Column;
//}

//struct Word
//{
//    public List<Token> Tokens;
    
//    public Word()
//    {
//        Tokens = new List<Token>();
//    }
    
//    public override string ToString() => string.Join("", Tokens.Select(x => x.Value));
    
//    public (int start, int end) Range => (Tokens.First().Column, Tokens.Last().Column);
    
//}

//struct Scope
//{
//    public string Owner;
//    public ScopeLevel Level;
//    public List<Word> Words;
//    public (int start, int end) Range => (Words.First().Range.start, Words.Last().Range.end);
//}

//enum ScopeLevel
//{
//    Global,
//    Class,
//    Function,
//    Block
//}