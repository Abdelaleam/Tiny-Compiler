using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiny_Compiler;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        bool noReturn;
        bool err;

        public Node StartParsing(List<Token> TokenStream)
        {

            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            TokenStream.RemoveAll(token => token.token_type == Token_Class.Comment);
            root = new Node("Program");
            root.Children.Add(Program());
            noReturn = true;
            err = false;
            return root;
        }

        private void printError(string Expected, int inputPointer = -1)
        {
            if (inputPointer == -1)
                inputPointer = InputPointer;
            if (inputPointer < TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                            + Expected + " and " +
                            TokenStream[inputPointer].token_type.ToString() +
                            "  found\r\n");
            }
            InputPointer++;
        }

        //private Node commenta()
        //{
        //    Node comment = new Node("comment");
        //    if (TokenStream[InputPointer].token_type == Token_Class.Comment)
        //    {
        //       comment.Children.Add(match(Token_Class.Comment));
        //        // Simply skip the comment token
              
        //        return comment;
        //    }
        //    return null;
        //}
        private bool isStatement(int InputPointer)
        {
            bool isDecleration = isDatatype(InputPointer);
            bool isWrite = TokenStream[InputPointer].token_type == Token_Class.WRITE;
            bool isRead = TokenStream[InputPointer].token_type == Token_Class.READ;
            bool isConditionOrFunctionCallOrAssignment = TokenStream[InputPointer].token_type == Token_Class.Identifier;
            bool isIf = TokenStream[InputPointer].token_type == Token_Class.IF;
            bool isRepeat = TokenStream[InputPointer].token_type == Token_Class.REPEAT;
            return (isDecleration || isWrite || isRead || isConditionOrFunctionCallOrAssignment || isIf || isRepeat);
        }

        private bool isDatatype(int InputPointer)
        {
            bool isInt = TokenStream[InputPointer].token_type == Token_Class.INTEGER;
            bool isFloat = TokenStream[InputPointer].token_type == Token_Class.FLOAT;
            bool isString = TokenStream[InputPointer].token_type == Token_Class.STRING;
            return (isInt || isFloat || isString);
        }

        private bool isConditionOp(int InputPointer)
        {
            bool isLessThan = TokenStream[InputPointer].token_type == Token_Class.LessThanOp;
            bool isGreaterThan = TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp;
            bool isEqual = TokenStream[InputPointer].token_type == Token_Class.EqualOp;
            bool isNotEqual = TokenStream[InputPointer].token_type == Token_Class.NotEqualOp;
            return (isEqual || isGreaterThan || isLessThan || isNotEqual);
        }
        // MARYAMMMM

        Node Program()
        {
        //     if (TokenStream[InputPointer].token_type == Token_Class.Comment)
        //{
        //    // Simply skip the comment token
        //    InputPointer++;
        //}
            Node program_var = new Node("Program");
            //program_var.Children.Add(commenta());
            program_var.Children.Add(Function_statements());
            program_var.Children.Add(Main_function());
            return program_var;
        }
        Node Function_statements()
        {
            ///Function_statements → Function_statement   Function_statements | ɛ
            Node Function_statements_var = new Node("Function_statements");
            if (InputPointer < TokenStream.Count)
            {

                if (TokenStream[InputPointer].token_type == Token_Class.INTEGER || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.STRING)
                {
                    ++InputPointer;
                    if (InputPointer < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer].token_type != Token_Class.MAIN)
                        {
                            --InputPointer;
                            Function_statements_var.Children.Add(Function_statement());
                            Function_statements_var.Children.Add(Function_statements());
                            return Function_statements_var;
                        }
                    }

                    --InputPointer;
                }
                else
                {
                    return null;
                }
            }
            return Function_statements_var;
        }
        Node Function_statement()
        {
            ///Function_Statement → Function_Declaration Function_Body
            Node Function_statement_var = new Node("Function_statement");
            Function_statement_var.Children.Add(Function_declaration());
            Function_statement_var.Children.Add(Function_body());
            return Function_statement_var;

        }
        Node Function_declaration()
        {
            Node Function_declaration_var = new Node("Function_declaration");
            Function_declaration_var.Children.Add(DataType());
            Function_declaration_var.Children.Add(function_name());
            Function_declaration_var.Children.Add(match(Token_Class.left_parenthesis));
            Function_declaration_var.Children.Add(Parameters());
            Function_declaration_var.Children.Add(match(Token_Class.right_parenthesis));
            return Function_declaration_var;
        }
        Node Function_body()
        {
            noReturn = true;

            Node funBody_var = new Node("function body");
            funBody_var.Children.Add(match(Token_Class.Left_Curly_Brace));
            funBody_var.Children.Add(Statements());
            if (noReturn)
            {
                funBody_var.Children.Add(return_statement());
                noReturn = true;
            }

            funBody_var.Children.Add(match(Token_Class.Right_Curly_Brace));
            return funBody_var;
        }
        Node return_statement()
        {
            Node return_statement_var = new Node("return statement");
            return_statement_var.Children.Add(match(Token_Class.RETURN));
            return_statement_var.Children.Add(Expression());
            return_statement_var.Children.Add(match(Token_Class.Semicolon));
            return return_statement_var;
        }
        //private Node Statement()
        //{
        //    Node statement = new Node("Statement");
        //    if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
        //    {
        //        // Assignment
        //        if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
        //        {
        //            statement.Children.Add(Ass_stmt());
        //            statement.Children.Add(match(Token_Class.Semicolon));
        //        }
        //        //Condition
        //        else if (InputPointer + 1 < TokenStream.Count && isConditionOp(InputPointer + 1))
        //        {
        //            statement.Children.Add(Condition_Statement());
        //        }
        //        //Function Call
        //        else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.left_parenthesis)
        //        {
        //            statement.Children.Add(Function_Call());
        //            statement.Children.Add(match(Token_Class.Semicolon));
        //        }
        //        else
        //            printError("Assignment or Condition or Function", InputPointer + 1);
        //    }
        //    // Declaration
        //    else if (InputPointer < TokenStream.Count && isDatatype(InputPointer))
        //    {
        //        statement.Children.Add(Declaration_Statement());
        //    }
        //    else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.WRITE)
        //    {
        //        statement.Children.Add(Write_Statement());
        //    }
        //    else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.READ)
        //    {
        //        statement.Children.Add(Read_Statement());
        //    }
        //    else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.IF)
        //    {
        //        statement.Children.Add(If_Statement());
        //    }
        //    else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.REPEAT)
        //    {
        //        statement.Children.Add(Repeat_statement());
        //    }
        //    //else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
        //    //{
        //    //    statement.Children.Add(match(Token_Class.Comment));
        //    //}

        //    else
        //        printError("Statement");
        //    return statement;
        //}
        private Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    // Assignment
                    if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
                    {
                        statement.Children.Add(Ass_stmt());
                        statement.Children.Add(match(Token_Class.Semicolon));
                    }
                    // Condition
                    else if (InputPointer + 1 < TokenStream.Count && isConditionOp(InputPointer + 1))
                    {
                        statement.Children.Add(Condition_Statement());
                    }
                    // Function Call
                    else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.left_parenthesis)
                    {
                        statement.Children.Add(Function_Call());
                        statement.Children.Add(match(Token_Class.Semicolon));
                    }
                    else
                        printError("Assignment, Condition, or Function Call", InputPointer + 1);
                }
                // Declaration
                else if (isDatatype(InputPointer))
                {
                    statement.Children.Add(Declaration_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.WRITE)
                {
                    statement.Children.Add(Write_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.READ)
                {
                    statement.Children.Add(Read_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.IF)
                {
                    statement.Children.Add(If_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.REPEAT)
                {
                    statement.Children.Add(Repeat_statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    // Simply skip the comment token
                    InputPointer++;
                }
                else
                {
                    printError("Statement");
                }
            }
            return statement;
        }
        private Node Statements()
        {
            Node statements = new Node("Statements");
            if (InputPointer < TokenStream.Count && isStatement(InputPointer))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(StatementsD());
                return statements;
            }
            else
                return null;
        }

        private Node StatementsD()
        {
            Node statementD = new Node("Statements'");
            if (InputPointer < TokenStream.Count && isStatement(InputPointer))
            {
                statementD.Children.Add(Statement());
                statementD.Children.Add(StatementsD());
                return statementD;
            }
            else
                return null;
        }


        private Node Parameters()
        {
            //Parameters → Parameter Parameters2 | ε

            Node parameters = new Node("parameters");

            if (InputPointer < TokenStream.Count)
            {
                bool isInteger = (TokenStream[InputPointer].token_type == Token_Class.INTEGER);
                bool isFloat = (TokenStream[InputPointer].token_type == Token_Class.FLOAT);
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.STRING);

                if (isInteger || isFloat || isString)
                {
                    parameters.Children.Add(Parameter());
                    parameters.Children.Add(Parameters2());
                    return parameters;
                }
                else
                    return null;
            }
            return null;
        }
        private Node Parameter()
        {
            //Parameter → Datatype Identifier
            Node parameter = new Node("parameter");

            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
        private Node Parameters2()
        {
            //Parameters2 →  “,”  Parameters | ε

            Node parameters2 = new Node("parameters2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    parameters2.Children.Add(match(Token_Class.Comma));
                    parameters2.Children.Add(Parameters());
                    return parameters2;
                }
                else
                    return null;
            }
            return null;

        }
        public Node function_name()
        {
            Node function_name_var = new Node("function_name");
            function_name_var.Children.Add(match(Token_Class.Identifier));
            return function_name_var;
        }
        Node Repeat_statement()
        {
            Node Repeat_statement_var = new Node("repeat statement");
            Repeat_statement_var.Children.Add(match(Token_Class.REPEAT));
            Repeat_statement_var.Children.Add(Statements());
            Repeat_statement_var.Children.Add(match(Token_Class.UNTIL));
            Repeat_statement_var.Children.Add(Condition_Statement());

            return Repeat_statement_var;
        }
        Node Main_function()
        {
            Node Main_function_var = new Node("main function");
            Main_function_var.Children.Add(DataType());
            Main_function_var.Children.Add(match(Token_Class.MAIN));
            Main_function_var.Children.Add(match(Token_Class.left_parenthesis));
            Main_function_var.Children.Add(match(Token_Class.right_parenthesis));
            Main_function_var.Children.Add(Function_body());
            return Main_function_var;
        }


///shahddd 


private Node Condition_Statement()
        {
            //Cond_Stmt → cond cond_stmt2
            Node cond_stmt = new Node("Cond_Stmt");
            cond_stmt.Children.Add(Condition());
            cond_stmt.Children.Add(Conditions());
            return cond_stmt;
        }
        private Node Conditions()
        {
            //Cond_Stmt2 → Bool_Cond Cond_Stmt2 | e
            Node cond_stmt2 = new Node("cond_stmt2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOp || TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    cond_stmt2.Children.Add(BooleanOp());
                    cond_stmt2.Children.Add(Condition());
                    cond_stmt2.Children.Add(Conditions());
                    return cond_stmt2;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public Node If_Statement()
        {
            //if_stat →  if Cond_Stmt then Statements else_if_stat else_stat end
            Node if_stat = new Node("if_stat");

            if_stat.Children.Add(match(Token_Class.IF));
            if_stat.Children.Add(Condition_Statement());
            if_stat.Children.Add(match(Token_Class.THEN));
            if_stat.Children.Add(Statements());
            while (true)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                    if_stat.Children.Add(Else_If_Statement());
                else
                    break;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                if_stat.Children.Add(Else_Statement());
            if_stat.Children.Add(match(Token_Class.END));


            return if_stat;
        }

        Node Else_If_Statement()
        {
            Node Else_If_Statement_var = new Node("Else_If_Statement");
            Else_If_Statement_var.Children.Add(match(Token_Class.ELSEIF));
            Else_If_Statement_var.Children.Add(Condition_Statement());
            Else_If_Statement_var.Children.Add(match(Token_Class.THEN));
            Else_If_Statement_var.Children.Add(Statements());

            return Else_If_Statement_var;

        }
        Node Else_Statement()
        {
            Node Else_Statement_var = new Node("Else_Statement");
            Else_Statement_var.Children.Add(match(Token_Class.ELSE));
            Else_Statement_var.Children.Add(Statements());
            return Else_Statement_var;
        }

        Node Return_Statement()
        {
            Node Return_Statement_var = new Node("Return_Statement");
            Return_Statement_var.Children.Add(match(Token_Class.RETURN));
            Return_Statement_var.Children.Add(Expression());
            Return_Statement_var.Children.Add(match(Token_Class.Semicolon));
            return Return_Statement_var;

        }
        Node Read_Statement()
        {
            Node Read_Statement_var = new Node("Read_Statement");
            Read_Statement_var.Children.Add(match(Token_Class.READ));
            Read_Statement_var.Children.Add(match(Token_Class.Identifier));
            Read_Statement_var.Children.Add(match(Token_Class.Semicolon));
            return Read_Statement_var;
        }
        Node Write_Statement()
        {
            Node Write_Statement_var = new Node("Write_Statement");
            Write_Statement_var.Children.Add(match(Token_Class.WRITE));
            Write_Statement_var.Children.Add(Write2());
            Write_Statement_var.Children.Add(match(Token_Class.Semicolon));
            return Write_Statement_var;
        }
        Node Write2()
        {
            Node Write2_var = new Node("Write_2");
            if (InputPointer < TokenStream.Count)
            {
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);

                bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                //bool isDigit = (TokenStream[InputPointer].token_type == Token_Class.Digit);
                bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);
                if (TokenStream[InputPointer].token_type == Token_Class.ENDL)
                {
                    Write2_var.Children.Add(match(Token_Class.ENDL));
                   // Write2_var.Children.Add(match(Token_Class.Semicolon));
                    return Write2_var;
                }
                else if (isString || isNumber || isIdnetifier)
                {
                    Write2_var.Children.Add(Expression());
                   // Write2_var.Children.Add(match(Token_Class.Semicolon));
                    return Write2_var;
                }
                else
                    printError("Write Statement");

            }

            return Write2_var;
        }

        public Node Declaration_Statement()
        {
            Node Declaration_Statement_var = new Node("Declaration_Statement");
            Declaration_Statement_var.Children.Add(DataType());
            Declaration_Statement_var.Children.Add(Identifiers());
            Declaration_Statement_var.Children.Add(match(Token_Class.Semicolon));
            return Declaration_Statement_var;

        }


        private Node Identifiers()
        {
            Node identifiers = new Node("Identifiers");
            if (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.Comma || TokenStream[InputPointer + 1].token_type == Token_Class.Semicolon))
            {
                identifiers.Children.Add(match(Token_Class.Identifier));
                identifiers.Children.Add(IdentifiersD());
            }
            else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
            {
                identifiers.Children.Add(Ass_stmt());
                identifiers.Children.Add(IdentifiersD());
            }
            return identifiers;
        }

        private Node IdentifiersD()
        {
            Node identifiersD = new Node("IDs'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                identifiersD.Children.Add(match(Token_Class.Comma));
                identifiersD.Children.Add(Identifiers());
                return identifiersD;
            }
            else
                return null;
        }


        public Node DataType()  //Datatype → int | float | string 
        {
            Node dataType = new Node("Datatype");

            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.INTEGER)
                {
                    dataType.Children.Add(match(Token_Class.INTEGER));
                    return dataType;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.FLOAT)
                {

                    dataType.Children.Add(match(Token_Class.FLOAT));
                    return dataType;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.STRING)
                {

                    dataType.Children.Add(match(Token_Class.STRING));
                    return dataType;
                }
            }

            return dataType;
        }

        public Node Ass_stmt()  ////assignment statement→ identifier := Expression
        {
            Node ass_stmt = new Node("Assigment stmt");

            ass_stmt.Children.Add(match(Token_Class.Identifier));
            ass_stmt.Children.Add(match(Token_Class.AssignmentOp));
            ass_stmt.Children.Add(Expression());
            return ass_stmt;
        }
        private Node Alone_Ass_Stmt() //assignment statement
        {
            //ass_stmt →  Identifier  :=  Experssion
            Node alone_Ass_Stmt = new Node(" Alone_Ass_Stmt");

            alone_Ass_Stmt.Children.Add(match(Token_Class.Identifier));
            alone_Ass_Stmt.Children.Add(match(Token_Class.AssignmentOp));
            alone_Ass_Stmt.Children.Add(Expression());
            alone_Ass_Stmt.Children.Add(match(Token_Class.Semicolon));

            return alone_Ass_Stmt;
        }
        public Node Expression()  //Expression → string | Term | Equation 
        {
            Node expression = new Node("Expression");

            if (InputPointer < TokenStream.Count)
            {
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);
                bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);
                //bool isDigit = (TokenStream[InputPointer].token_type == Token_Class.Digit);

                if (isString)
                {
                    expression.Children.Add(match(Token_Class.String));
                    return expression;
                }
                if (isNumber || isIdnetifier ) //term or equation
                {
                    ++InputPointer;
                    if (InputPointer < TokenStream.Count)
                    {
                        bool isPlusOp = (TokenStream[InputPointer].token_type == Token_Class.PlusOp);
                        bool isMinusOp = (TokenStream[InputPointer].token_type == Token_Class.MinusOp);
                        bool isMultiplyOp = (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp);
                        bool isDivideOp = (TokenStream[InputPointer].token_type == Token_Class.DivideOp);

                        if (isPlusOp || isMinusOp || isMultiplyOp || isDivideOp)
                        {
                            --InputPointer;
                            expression.Children.Add(Equation());
                            return expression;
                        }
                    }
                    --InputPointer;
                    expression.Children.Add(Term());
                    return expression;
                }
                else
                    printError("Expression");

            }
            return expression;
        }

        public Node Arithmatic_Op()  //+ | - | * | \
        {
            Node op = new Node("op");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    op.Children.Add(match(Token_Class.PlusOp));
                    return op;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    op.Children.Add(match(Token_Class.MinusOp));
                    return op;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    op.Children.Add(match(Token_Class.MultiplyOp));
                    return op;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    op.Children.Add(match(Token_Class.DivideOp));
                    return op;
                }

            }
            return op;
        }

        private Node BooleanOp()
        {
            Node boolOp = new Node("Boolean Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                boolOp.Children.Add(match(Token_Class.AndOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                boolOp.Children.Add(match(Token_Class.OrOp));
            }
            else
                printError("Boolean Operator");
            return boolOp;
        }
        public Node Condition_Op()   //cond_op →    < |   > |   = | <>

        {
            Node condi = new Node("condition_Operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                {
                    //>
                    condi.Children.Add(match(Token_Class.LessThanOp));
                    return condi;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                {
                    //<
                    condi.Children.Add(match(Token_Class.GreaterThanOp));
                    return condi;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                {
                    //=
                    condi.Children.Add(match(Token_Class.EqualOp));
                    return condi;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
                {
                    //<>
                    condi.Children.Add(match(Token_Class.NotEqualOp));
                    return condi;
                }
                else
                    printError("condtion op");
            }
            return condi;
        }
        public Node Function_Call()  //Function_Call → Function_Name ( Arguments )
        {
            Node func_call = new Node("Function Call");
            func_call.Children.Add(function_name()); 
            func_call.Children.Add(match(Token_Class.left_parenthesis));
            func_call.Children.Add(Arugments());
            func_call.Children.Add(match(Token_Class.right_parenthesis));
            return func_call;
        }

        public Node Arugments() //args → Identifier  args2  |  𝜀
        {

            Node args_var = new Node("arguments");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    args_var.Children.Add(match(Token_Class.Identifier));
                    args_var.Children.Add(arg2());
                    return args_var;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    args_var.Children.Add(match(Token_Class.Number));
                    args_var.Children.Add(arg2());
                    return args_var;
                }
                //if (TokenStream[InputPointer].token_type == Token_Class.Digit)
                //{
                //    args_var.Children.Add(match(Token_Class.Digit));
                //    args_var.Children.Add(arg2());
                //    return args_var;
                //}
                else
                {
                    return null;
                }
            }
            return args_var;
        }

        private Node arg2() //args2 →   , Identifier  args2  |   𝜀
        {

            Node args2_var = new Node("args2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    args2_var.Children.Add(match(Token_Class.Comma));
                    args2_var.Children.Add(match(Token_Class.Identifier));
                    args2_var.Children.Add(arg2());
                    return args2_var;
                }
                else
                {
                    return null;
                }
            }
            return args2_var;
        }

        public Node Condition() // Condition → identifier Condition_Op Term
        {
            Node cond = new Node("Condition");
            cond.Children.Add(match(Token_Class.Identifier));
            cond.Children.Add(Condition_Op());
            cond.Children.Add(Term());
            return cond;

        }
        
        public Node Term()
        {
            //Term → number | identifier | function_call
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count)
            {
                //if (TokenStream[InputPointer].token_type == Token_Class.Digit)
                //{
                //    term.Children.Add(match(Token_Class.Digit));
                //    return term;
                //}
                 if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    
                    
                    term.Children.Add(match(Token_Class.Number));
                    return term;
                }

                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier) //AMBUGUITY
                {
                    ++InputPointer;
                    if (InputPointer < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer].token_type == Token_Class.left_parenthesis)
                        {
                            --InputPointer;
                            term.Children.Add(Function_Call());
                            return term;
                        }
                    }

                    --InputPointer;
                    term.Children.Add(match(Token_Class.Identifier));
                    return term;

                }
                else
                {
                    printError("Term");
                }
            }
        
            return term;

        }
        public Node Equation() //Equation → Equation2 op Equation2 Equation3

        {
            Node equation = new Node("equation");
            equation.Children.Add(Equation2());
            equation.Children.Add(Arithmatic_Op());
            equation.Children.Add(Equation2());
            equation.Children.Add(Equation3());
            return equation;
        }

        public Node Equation2()   //Equation2 → Term | (Equation) 
        {

            Node equation2 = new Node("equation2");
            if (InputPointer < TokenStream.Count)
            {
                bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);
                bool isParanthesis = (TokenStream[InputPointer].token_type == Token_Class.left_parenthesis);
                 
                if (isNumber || isIdnetifier ) //if it is term
                {
                    equation2.Children.Add(Term());
                    return equation2;
                }
                else if (isParanthesis)
                {
                    equation2.Children.Add(match(Token_Class.left_parenthesis));
                    equation2.Children.Add(Equation());
                    equation2.Children.Add(match(Token_Class.right_parenthesis));
                    return equation2;
                }
            }

            return equation2;
        }

        private Node Equation3()  //Equation3 → op Eqaution | ε
        {


            Node equation3 = new Node("equation3");
            if (InputPointer < TokenStream.Count)
            {
                bool isPlusOp = (TokenStream[InputPointer].token_type == Token_Class.PlusOp);
                bool isMinusOp = (TokenStream[InputPointer].token_type == Token_Class.MinusOp);
                bool isMultiplyOp = (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp);
                bool isDivideOp = (TokenStream[InputPointer].token_type == Token_Class.DivideOp);


                if (isPlusOp || isMinusOp || isMultiplyOp || isDivideOp)
                {
                    equation3.Children.Add(Arithmatic_Op());
                    equation3.Children.Add(Equation());
                    return equation3;
                }
                else
                    return null;
            }
            return null;

        }
        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }
        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    } 
}
