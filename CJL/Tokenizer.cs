using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CJL;

public static class Tokenizer
{
   
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();

        var lines = input.Split(Environment.NewLine).ToList();
        int lineNum = -1;

        bool foundAppDec = false;
        bool foundEntryDec = false;
        foreach (var line in lines)
        {
            lineNum++;
            if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("//"))
                continue;
            var withoutTab = line.TrimStart('\t');
            int firstNonTab = line.IndexOf(withoutTab);
            
            int scopeLevel = firstNonTab == 0 || firstNonTab == -1 ? 0 : line.Substring(0, firstNonTab).Count(t=>t == '\t');
            
            //find app declaration
            if (!foundAppDec)
            {
                if (!line.Trim().StartsWith("app"))
                {
                    throw new Exception("App declaration not found");
                }
                else
                {
                    foundAppDec = true;
                    var token = new Token(line.Trim()[..3],TokenType.AppDeclaration, lineNum, 0, scopeLevel);
                    tokens.Add(token);

                    var appName = line.Trim().Split(new char[] { ' ', ':'})[1];
                    if (char.IsLetter(appName[0]) == false)
                        throw new Exception("App name must start with a letter");
                    var appNametoken = new Token(appName, TokenType.AppNameDeclaration, lineNum, 4, scopeLevel);
                    tokens.Add(appNametoken);

                    var appScope = line.Trim().Last();
                    if (appScope != ':')
                        throw new Exception("App scope definition ':' not found");
                    var appScopeToken = new Token(appScope.ToString(), TokenType.ScopeStart, lineNum, line.IndexOf(appScope), scopeLevel);
                    tokens.Add(appScopeToken);
                }

                continue;
            }

            //find entry
            if (!foundEntryDec)
            {
                if (!line.StartsWith("\tentry"))
                {
                    throw new Exception("entry not found in app scope");
                }
                else
                {
                    foundEntryDec = true;
                    var token = new Token(
                        "entry", 
                        TokenType.EntryDeclaration, 
                        lineNum, 
                        4, 
                        scopeLevel);
                    tokens.Add(token);

                    var entryParamStart = line["\tentry".Length..].Trim()[0];
                    if (entryParamStart != '(')
                        throw new Exception("Entry paramater start '(' not found");

                    var entryParamStartToken = new Token("(", TokenType.EntryParamsStart, lineNum, line.IndexOf('('), scopeLevel);
                    tokens.Add(entryParamStartToken);

                    var entryParams = line[(line.IndexOf('(') + 1)..line.IndexOf(')')]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var entryParam in entryParams)
                    {
                        var spl = entryParam.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var chunk in spl)
                        {
                            var parts = chunk.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            //todo: validate first part is a type declaration
                            var entryParmTypeDec = parts[0];
                            var entryParmNameDec = parts[1];

                            var entryParmTypeTok = new Token(
                                entryParmTypeDec, 
                                TokenType.EntryParamTypeDeclaration, 
                                lineNum, 
                                line.IndexOf(entryParmTypeDec), 
                                scopeLevel);
                            tokens.Add(entryParmTypeTok);

                            var entryParmNameTok = new Token(
                                entryParmNameDec, 
                                TokenType.EntryParamNameDeclaration, 
                                lineNum, 
                                line.IndexOf(entryParmNameDec), 
                                scopeLevel);
                            tokens.Add(entryParmNameTok);
                        }
                    }

                    var entryParamEnd = line.Trim().Last(t=>t != ':');
                    if (entryParamEnd != ')')
                        throw new Exception("Entry paramater start '(' not found");

                    var entryParamEndToken = new Token(")", TokenType.EntryParamsEnd, lineNum, line.IndexOf(')'), scopeLevel);
                    tokens.Add(entryParamEndToken);

                    var entryScope = line.Trim().Last();
                    if (entryScope != ':')
                        throw new Exception("entry scope start ':' not found");

                    var appScopeToken = new Token(entryScope.ToString(), TokenType.ScopeStart, lineNum, line.IndexOf(entryScope), scopeLevel);
                    tokens.Add(appScopeToken);

                }

                continue;
            }

            //find classes
            if (line.Trim().StartsWith("class"))
            {
                var token = new Token("class", TokenType.ClassDeclaration, lineNum, scopeLevel * 4, scopeLevel);
                tokens.Add(token);

                var className = line.Trim().Split(new char[] { ' ', ':', '('})[1];
                
                if (char.IsLetter(className[0]) == false)
                    throw new Exception("Class name must start with a letter");
                
                var classNameToken = 
                    new Token(
                        className, 
                        TokenType.ClassNameDeclaration, 
                        lineNum,scopeLevel * 4 + "class".Length + 1, 
                        scopeLevel);
                tokens.Add(classNameToken);

                var classConstructorParamStart = line.Trim()["class".Length + className.Length + 1];
                if (classConstructorParamStart != '(')
                    throw new Exception("Class constructor paramater start '(' not found");

                var classConstructorParamStartToken = new Token("(", TokenType.ClassConstructorParamsStart, lineNum, line.IndexOf('('), scopeLevel);
                tokens.Add(classConstructorParamStartToken);

                var classConstructorParams = line[(line.IndexOf('(') + 1)..line.IndexOf(')')]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var classConstructorParam in classConstructorParams)
                {
                    var spl = classConstructorParam.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var chunk in spl)
                    {
                        var parts = chunk.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        //todo: validate first part is a type declaration
                        var classConstParmTypeDec = parts[0];
                        var classConstParmNameDec = parts[1];

                        var classConstParmTypeTok = new Token(
                            classConstParmTypeDec,
                            TokenType.ClassConstructorParamTypeDeclaration,
                            lineNum,
                            line.IndexOf(classConstParmTypeDec),
                            scopeLevel);
                        tokens.Add(classConstParmTypeTok);

                        var classConstParmNameTok = new Token(
                            classConstParmNameDec,
                            TokenType.ClassConstructorParamNameDeclaration,
                            lineNum,
                            line.IndexOf(classConstParmNameDec),
                            scopeLevel);
                        tokens.Add(classConstParmNameTok);

                    }
                }


                var classConstParamEnd = line.Trim().Last(t => t != ':');
                if (classConstParamEnd != ')')
                    throw new Exception("class constructor paramater end ')' not found");
                
                var classConstParamEndToken = new Token(")", TokenType.ClassContructorParamsEnd, lineNum, line.IndexOf(')'), scopeLevel);
                tokens.Add(classConstParamEndToken);

                var classScope = line.Trim().Last();
                if (classScope != ':')
                    throw new Exception("Class scope start ':' not found");
                
                var classScopeToken = new Token(
                    classScope.ToString(), 
                    TokenType.ScopeStart, 
                    lineNum, 
                    line.IndexOf(classScope), 
                    scopeLevel);
                tokens.Add(classScopeToken);

                continue;
            }


            //find functions
            var typeNames = ClassNames(tokens).ToList();
            typeNames.AddRange(new List<string>
            {
                "int",
                "float",
                "bool",
                "string",
                "char",
                "void"
            });
            if (StartsWith(line.Trim(), typeNames.ToArray()) && line.Trim().EndsWith(":"))
            {
                var type = line.Trim().Split(' ')[0];
                var token = new Token(type, TokenType.FunctionTypeDeclaration, lineNum, scopeLevel * 4, scopeLevel);
                tokens.Add(token);

                var functionName = line.Trim().Split(new char[] { ' ', '('})[1];
                if (char.IsLetter(functionName[0]) == false)
                    throw new Exception("Function name must start with a letter");
                var functionNameToken = new Token(functionName, TokenType.FunctionNameDeclaration, lineNum, scopeLevel * 4 + type.Length + 1, scopeLevel);
                tokens.Add(functionNameToken);

                var functionParamStart = line.Trim()[line.Trim().IndexOf('(')];
                if (functionParamStart != '(')
                    throw new Exception("Function paramater start '(' not found");
                var functionParamStartToken = new Token("(", TokenType.FunctionParamsStart, lineNum, line.IndexOf('('), scopeLevel);
                tokens.Add(functionParamStartToken);

                var functionParams = line[(line.IndexOf('(') + 1)..line.IndexOf(')')]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var functionParam in functionParams)
                {
                    var spl = functionParam.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var chunk in spl)
                    {
                        var parts = chunk.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        //todo: validate first part is a type declaration
                        var functionParmTypeDec = parts[0];
                        var functionParmNameDec = parts[1];

                        var functionParmTypeTok = new Token(
                            functionParmTypeDec,
                            TokenType.FunctionParamTypeDeclaration,
                            lineNum,
                            line.IndexOf(functionParmTypeDec),
                            scopeLevel);
                        tokens.Add(functionParmTypeTok);

                        var functionParmNameTok = new Token(
                            functionParmNameDec,
                            TokenType.FunctionParamNameDeclaration,
                            lineNum,
                            line.IndexOf(functionParmNameDec),
                            scopeLevel);
                        tokens.Add(functionParmNameTok);
                    }
                }

                var functionParamEnd = line.Trim().Last(t => t != ':');
                if (functionParamEnd != ')')
                    throw new Exception("Function paramater end ')' not found");
                var functionParamEndToken = new Token(")", TokenType.FunctionParamsEnd, lineNum, line.IndexOf(')'), scopeLevel);

                tokens.Add(functionParamEndToken);

                var functionScope = line.Trim().Last();
                if (functionScope != ':')
                    throw new Exception("Function scope start ':' not found");
                var functionScopeToken = new Token(functionScope.ToString(), TokenType.ScopeStart, lineNum, line.IndexOf(functionScope), scopeLevel);
                tokens.Add(functionScopeToken);

                continue;
            }

            //find statements
            var statement = line.Trim();
            var statementToken = new Token(statement, TokenType.Statement, lineNum, scopeLevel * 4, scopeLevel);
            tokens.Add(statementToken);
        }

        return tokens;
    }

    private static bool StartsWith(string val, params string[] target)
    {
        foreach (var t in target)
        {
            if (val.StartsWith(t))
                return true;
        }

        return false;
    }

    private static string[] ClassNames(List<Token> tokens) =>
        tokens.Where(t => t.TokenType == TokenType.ClassNameDeclaration).Select(t => t.Value).ToArray();
    
}
    



public class Token
{
    public string Value { get; set; }
    
    public TokenType TokenType { get; set; }

    public int Line { get; set; }

    public int Column { get; set; }

    public int ScopeLevel { get; set; }

    public Token(string value, TokenType tokenType, int line, int column, int scopeLevel)
    {
        Value = value;
        TokenType = tokenType;
        Line = line;
        Column = column;
        ScopeLevel = scopeLevel;
    }
}

public enum TokenType
{
    AppDeclaration,                         //app
    AppNameDeclaration,                     //TestApp
    
    ScopeStart,                             //:
    
    EntryDeclaration,                       //entry
    EntryParamsStart,                       //(
    EntryParamsEnd,                         //)
    EntryParamTypeDeclaration,              //string[]
    EntryParamNameDeclaration,              //args
    EntryParamSeperator,                    //,
    
    ClassDeclaration,                       //class
    ClassNameDeclaration,                   //ArgPrinter
    ClassConstructorParamsStart,            //(
    ClassContructorParamsEnd,               //)
    ClassConstructorParamTypeDeclaration,   //int
    ClassConstructorParamNameDeclaration,   //age
    ClassConstructorParamSeperator,         //,
    
    FunctionTypeDeclaration,                //void
    FunctionNameDeclaration,                //PrintArgs
    FunctionParamsStart,                    //(
    FunctionParamsEnd,                      //)
    FunctionParamTypeDeclaration,           //float
    FunctionParamNameDeclaration,           //weight

    Statement,                              //print("Hello World")    
}