using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GR.DynamicEntityStorage.Utils
{
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }


        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }


        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }


        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        class SubtreeEvaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;

            public HashSet<Expression> Candidates
            {
                get => candidates;
                set => candidates = value;
            }

            public HashSet<Expression> Candidates1
            {
                get => candidates;
                set => candidates = value;
            }

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.Candidates = candidates;
            }


            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }


            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }


                if (this.Candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }


                return base.Visit(exp);
            }


            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }


                LambdaExpression lambda = Expression.Lambda(e);


                Delegate fn = lambda.Compile();


                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }


        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        class Nominator : ExpressionVisitor
        {
            Func<Expression, bool> fnCanBeEvaluated;


            HashSet<Expression> candidates;


            bool cannotBeEvaluated;

            public Func<Expression, bool> FnCanBeEvaluated
            {
                get => fnCanBeEvaluated;
                set => fnCanBeEvaluated = value;
            }

            public HashSet<Expression> Candidates
            {
                get => candidates;
                set => candidates = value;
            }

            public bool CannotBeEvaluated
            {
                get => cannotBeEvaluated;
                set => cannotBeEvaluated = value;
            }

            public Func<Expression, bool> FnCanBeEvaluated1
            {
                get => fnCanBeEvaluated;
                set => fnCanBeEvaluated = value;
            }

            public HashSet<Expression> Candidates1
            {
                get => candidates;
                set => candidates = value;
            }

            public bool CannotBeEvaluated1
            {
                get => cannotBeEvaluated;
                set => cannotBeEvaluated = value;
            }

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.FnCanBeEvaluated = fnCanBeEvaluated;
            }


            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.Candidates = new HashSet<Expression>();


                this.Visit(expression);


                return this.Candidates;
            }


            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.CannotBeEvaluated;


                    this.CannotBeEvaluated = false;


                    base.Visit(expression);


                    if (!this.CannotBeEvaluated)
                    {
                        if (this.FnCanBeEvaluated(expression))
                        {
                            this.Candidates.Add(expression);
                        }


                        else
                        {
                            this.CannotBeEvaluated = true;
                        }
                    }


                    this.CannotBeEvaluated |= saveCannotBeEvaluated;
                }


                return expression;
            }
        }
    }
}
