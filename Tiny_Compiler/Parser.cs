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

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        // MARYAMMMM

        Node Program()
        {
            Node program_var = new Node("Program");
            program_var.Children.Add(Function_statements());
           //program_var.Children.Add(Main_function());
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
                        --InputPointer;
                        if (TokenStream[InputPointer].token_type != Token_Class.MAIN)
                        {
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
            Node funBody_var = new Node("function body");
            funBody_var.Children.Add(match(Token_Class.Left_Curly_Brace));
            funBody_var.Children.Add(Statements());
            funBody_var.Children.Add(return_statement());
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
        private Node Statements() //statements
        {
            //Statements  → Statement Statements2 

            Node statements = new Node("statements");
            statements.Children.Add(Statement());
            statements.Children.Add(statmentsDash());
            return statements;

        }
        Node statmentsDash()
        {

            Node statementsVAR = new Node("statementsDash");
            if (InputPointer < TokenStream.Count)
            {
                bool isInteger = (TokenStream[InputPointer].token_type == Token_Class.INTEGER);
                bool isFloat = (TokenStream[InputPointer].token_type == Token_Class.FLOAT);
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.STRING);
                bool isRead = (TokenStream[InputPointer].token_type == Token_Class.READ);
                bool isWrite = (TokenStream[InputPointer].token_type == Token_Class.WRITE);
                bool isRepeat = (TokenStream[InputPointer].token_type == Token_Class.REPEAT);
                bool isIf = (TokenStream[InputPointer].token_type == Token_Class.IF);
                bool isIdentifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);


                bool isStatement = isInteger || isFloat || isString || isRead || isWrite ||
                                    isRepeat || isIf || isIdentifier;


                if (isStatement)
                {
                    statementsVAR.Children.Add(Statements());
                    return statementsVAR;
                }
                return null;
            }
            else
                return null;
            
        }
        private Node Statement()
        {
            Node statement = new Node("statement");
            if (InputPointer < TokenStream.Count)
            {
                bool isInteger = (TokenStream[InputPointer].token_type == Token_Class.INTEGER);
                bool isFloat = (TokenStream[InputPointer].token_type == Token_Class.FLOAT);
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.STRING);

                if (isInteger || isFloat || isString)
                {
                    statement.Children.Add(Declaration_Statement());
                    return statement;
                }
                //AMBIGUITY
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier) //Ass_Stmt() or Cond()
                {
                    ++InputPointer; //i dont know i just did what i thought was right
                    if (InputPointer < TokenStream.Count)
                    {
                        bool isLessOp = (TokenStream[InputPointer].token_type == Token_Class.LessThanOp);
                        bool isGreatOp = (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp);
                        bool isEqualOp = (TokenStream[InputPointer].token_type == Token_Class.EqualOp);
                        bool isNotEqualOp = (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp);
                        bool isAssignmentOp = (TokenStream[InputPointer].token_type == Token_Class.AssignmentOp);


                        if (isAssignmentOp)
                        {
                            --InputPointer;
                            statement.Children.Add(Alone_Ass_Stmt());
                            return statement;
                        }
                        if (isLessOp || isGreatOp || isEqualOp || isNotEqualOp)
                        {
                            --InputPointer;
                            statement.Children.Add(Condition());
                            return statement;
                        }
                    }
                    --InputPointer;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.READ)
                {
                    statement.Children.Add(Read_Statement());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.WRITE)
                {
                    statement.Children.Add(Write_Statement());
                    return statement;
                }

                if (TokenStream[InputPointer].token_type == Token_Class.REPEAT)
                {
                    statement.Children.Add(Repeat_statement());
                    return statement;
                }
                /*HERE*/
                if (TokenStream[InputPointer].token_type == Token_Class.RETURN)
                {
                    bool noReturn = false;
                    statement.Children.Add(Return_Statement());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.IF)
                {
                    statement.Children.Add(If_Statement());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    statement.Children.Add(Else_If_Statement());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                {
                    statement.Children.Add(Else_Statement());
                    return statement;
                }
            }


            return statement;

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
            cond_stmt.Children.Add(Condition_Statements2());
            return cond_stmt;
        }
        private Node Condition_Statements2()
        {
            //Cond_Stmt2 → Bool_Cond Cond_Stmt2 | e
            Node cond_stmt2 = new Node("cond_stmt2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOp || TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    cond_stmt2.Children.Add(Bool_Cond());
                    cond_stmt2.Children.Add(Condition_Statements2());
                    return cond_stmt2;
                }
                else
                {
                    return null;
                }
            }
            return cond_stmt2;
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
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.STRING);
                bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);
                if (TokenStream[InputPointer].token_type == Token_Class.ENDL)
                {
                    Write2_var.Children.Add(match(Token_Class.ENDL));
                    Write2_var.Children.Add(match(Token_Class.Semicolon));
                    return Write2_var;
                }
                if (isString || isNumber || isIdnetifier)
                {
                    Write2_var.Children.Add(Expression());
                    Write2_var.Children.Add(match(Token_Class.Semicolon));
                    return Write2_var;
                }


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
        public Node Identifiers()  //Identifiers → Iden3 Iden2 
        {
            Node Identifiers_var = new Node("Identifiers");
            Identifiers_var.Children.Add(Iden3());
            Identifiers_var.Children.Add(Iden2());
            return Identifiers_var;
        }


        private Node Iden2()   //Iden2 → , Identifiers | ε
        {
            Node Iden2_var = new Node("Iden2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    Iden2_var.Children.Add(match(Token_Class.Comma));
                    Iden2_var.Children.Add(Identifiers());
                    return Iden2_var;
                }
                else
                {
                    return null;

                }
            }
            return Iden2_var;

        }

        private Node Iden3()   //Iden3 → Identifier  | Assignment_Statement
        {
            Node Iden3 = new Node("Iden3");

            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    ++InputPointer;
                    if (InputPointer < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer].token_type == Token_Class.AssignmentOp)
                        {
                            --InputPointer;
                            Iden3.Children.Add(Ass_stmt());
                            return Iden3;
                        }
                    }
                    --InputPointer;
                    Iden3.Children.Add(match(Token_Class.Identifier));
                    return Iden3;

                }

            }
            return Iden3;

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


                if (isString)
                {
                    expression.Children.Add(match(Token_Class.String));
                    return expression;
                }
                if (isNumber || isIdnetifier || TokenStream[InputPointer].token_type == Token_Class.Digit ) //term or equation
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

        public Node Boolean_Op()   //Boolean_Op →  "&&" | "||" 
        {
            Node bool_oper = new Node("Boolean_Op");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
                {
                    bool_oper.Children.Add(match(Token_Class.AndOp));
                    return bool_oper;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    bool_oper.Children.Add(match(Token_Class.OrOp));
                    return bool_oper;
                }
            }
            return bool_oper;
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
        public Node Bool_Cond()
        {
            //Bool_Cond → Bool_Op Cond
            Node bool_Cond = new Node("Bool_Cond");
            bool_Cond.Children.Add(Boolean_Op());
            bool_Cond.Children.Add(Condition());
            return bool_Cond;
        }
        public Node Term()
        {
            //Term → number | identifier | function_call
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Digit)
                {
                    term.Children.Add(match(Token_Class.Digit));
                    return term;
                }
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

                if (isNumber || isIdnetifier || TokenStream[InputPointer].token_type == Token_Class.Digit) //if it is term
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
