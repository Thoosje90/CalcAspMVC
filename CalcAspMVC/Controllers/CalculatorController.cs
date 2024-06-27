using CalcAspMVC.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CalcAspMVC.Controllers
{
    public class CalculatorController : Controller
    {
        // IHttpContextAccessor Interface for managing sessions
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CalculatorController(IHttpContextAccessor httpContextAccessor)
        {
            // Initialize IHttpContextAccessor for managing sessions
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Calculator", new Models.CalculatorModel());
        }

        #region Calculator Events

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate([FromBody] Models.CalculatorModel model)
        {
            // Validate Model
            if (model == null)
                return DefaultJsonObject();

            // Check for Zero Division
            if (IsDividingByZero(model.Expression))
            {
                // Totally unessary error but i like it
                // Update Result Notification
                model.Result = "Error : Cannot Devide By Zero";
            }
            else
            {
                try
                {
                    // Calculate expression using DataTable
                    object result = new DataTable().Compute(model.Expression, string.Empty);

                    if (result != DBNull.Value)
                    {
                        // Reset Result
                        model.Result = "";
                        // Update Exppression
                        model.Expression = result?.ToString() ?? "";
                    }
                }
                catch
                {
                    // Catch errors an update result
                    model.Result = "Error : Illegal Exppresion";
                }
            }

            // Get user session
            ISession? session = createSession();
            // Return Json Object
            return session == null ? DefaultJsonObject(model) : Json(new
            {
                Expression = model.Expression,
                Result = model.Result,
                Calculations = session.GetList()
            });
        }

        #endregion

        #region Memory Manager Events

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MemoryClear([FromBody] Models.CalculatorModel model)
        {
            // Get user session
            ISession? session = createSession();
            // Validate Model
            if (model == null)
                return DefaultJsonObject();
            // Validate session
            if (session == null)
                return DefaultJsonObject(model);

            // Get float list from user session
            List<float> memoryStore = session.GetList();

            // Remove all memory entries if index not found in model
            if (string.IsNullOrEmpty(model.MemoryIndex))
            {
                // Clear all memory entries
                session.SetList(new List<float>());
            }
            else
            {
                // Memory entry index
                int index;

                // Remove entry from index
                // Try parsing the MemoryIndex string from model to an integer
                if (int.TryParse(model.MemoryIndex, out index))
                    memoryStore.RemoveAt(index);

                // Update User Session
                session.SetList(memoryStore);
            }

            // Return Json Model
            return Json(new
            {
                Expression = model.Expression,
                Result = model.Result,
                Calculations = session.GetList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MemoryRecal([FromBody] Models.CalculatorModel model)
        {
            // Get user session
            ISession? session = createSession();
            // Validate Model
            if (model == null)
                return DefaultJsonObject();
            // Validate session
            if (session == null)
                return DefaultJsonObject(model);

            // Get Memory List from User Session
            List<float> memoryStore = session.GetList();

            // Check if MemoryStory has entries
            if (memoryStore.Count > 0)
            {
                // Get last item index
                int index = memoryStore.Count - 1; 
                // Update model with the latest entry from the Memory
                model.Expression = memoryStore[index].ToString();
            }

            // Return Json Model
            return Json(new
            {
                Expression = model.Expression,
                Result = model.Result,
                Calculations = memoryStore
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MemoryStore([FromBody] Models.CalculatorModel model)
        {
            // Get user session
            ISession? session = createSession();
            // Validate Model
            if (model == null)
                return DefaultJsonObject();
            // Validate session
            if (session == null)
                return DefaultJsonObject(model);

            // Get Memory List from User Session
            List<float> memoryStore = session.GetList();

            try
            {
                // Get expression with all calculations from the display (model)
                string expression = model.Expression;

                // Check if expression string not empty
                if (!string.IsNullOrEmpty(expression))
                {
                    // Remove all operators from expession
                    string[] operators = new string[] { "*", "+", "-", "/" };
                    foreach (string op in operators)
                        expression = expression.Replace(op, " ");

                    // Breakup all numbers and operatorions
                    string[] expressionArray = expression.Split(new char[0]);
                    // Get the last number from the expressionArray (if any)
                    float lastDigit = expressionArray.Length > 0 ? 
                        float.Parse(expressionArray[expressionArray.Length - 1]) : 
                        float.Parse(expression);

                    // Update memory store
                    memoryStore.Add(lastDigit);
                    // Update memery store in User Session
                    session.SetList(memoryStore);
                }
            }
            catch(Exception ex) 
            {
                // Update Result Notification
                model.Result = "Error : Illegal Exppresion";
            }

            // Return Json Model
            return Json(new { 
                Expression = model.Expression, 
                Result = model.Result, 
                Calculations = memoryStore 
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MemoryAdd([FromBody] Models.CalculatorModel model)
        {
            // Get user session
            ISession? session = createSession();        
            // Validate Model
            if (model == null)
                return DefaultJsonObject();
            // Validate session
            if (session == null)
                return DefaultJsonObject(model);

            // Get Memory List from User Session
            List<float> memoryStore = session.GetList();

            try
            {
                // Get expression with all calculations from the display (model)
                string expression = model.Expression;

                // Check if expression is not empty
                if (!string.IsNullOrEmpty(expression))
                {
                    // Remove Operators from Expression
                    string[] operators = new string[] { "*", "+", "-", "/" };
                    foreach (string op in operators)
                        expression = expression.Replace(op, " ");

                    // Breakup all numbers and operations
                    string[] expressionArray = expression.Split(new char[0]);
                    // Get the last number from the expressionArray (if any)
                    float lastDigit = expressionArray.Length > 0 ?
                        float.Parse(expressionArray[expressionArray.Length - 1]) :
                        float.Parse(expression);

                    // Check if MemoryStory has entries
                    if (memoryStore.Count > 0)
                    {
                        // Get last item index
                        int index = memoryStore.Count - 1;
                        // Try parsing Memory Index from model if found
                        if (!string.IsNullOrEmpty(model.MemoryIndex))
                        {
                            int.TryParse(model.MemoryIndex, out index);
                        }

                        // Update Memory Store
                        memoryStore[index] += lastDigit;
                    }

                    // Update Memory Store in User Session
                    session.SetList(memoryStore);
                }
            }
            catch (Exception ex)
            {
                // Update Result Notification
                model.Result = "Error : Illegal Exppresion";
            }

            // Return Json Model
            return Json(new
            {
                Expression = model.Expression,
                Result = model.Result,
                Calculations = memoryStore
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MemorySubstract([FromBody] Models.CalculatorModel model)
        {
            // Get user session
            ISession? session = createSession();
            // Validate Model
            if (model == null)
                return DefaultJsonObject();
            // Validate session
            if (session == null)
                return DefaultJsonObject(model);

            // Get Memory List from User Session
            List<float> memoryStore = session.GetList();

            try
            {
                // Get expression with all calculations from the display (model)
                string expression = model.Expression;

                // Check if string not empty
                if (!string.IsNullOrEmpty(expression))
                {
                    // Remove Operators from Expression
                    string[] operators = new string[] { "*", "+", "-", "/" };
                    foreach (string op in operators)
                        expression = expression.Replace(op, " ");

                    // Breakup all numbers and operatorions
                    string[] expressionArray = expression.Split(new char[0]);
                    // Get the last number from the expressionArray (if any)
                    float lastDigit = expressionArray.Length > 0 ?
                        float.Parse(expressionArray[expressionArray.Length - 1]) :
                        float.Parse(expression);

                    // Check if MemoryStory has entries
                    if (memoryStore.Count > 0)
                    {
                        // Get last item index
                        int index = memoryStore.Count - 1;
                        // Try parsing Memory Index from model if found
                        if (!string.IsNullOrEmpty(model.MemoryIndex))
                            int.TryParse(model.MemoryIndex, out index);

                        // Update Memory Store
                        memoryStore[index] -= lastDigit;
                    }

                    // Update Memory Store in User Session
                    session.SetList(memoryStore);
                }
            }
            catch (Exception ex)
            {
                // Update Result Notification
                model.Result = "Error : Illegal Exppresion";
            }

            // Return Json Model
            return Json(new
            {
                Expression = model.Expression,
                Result = model.Result,
                Calculations = memoryStore
            });
        }

        #endregion

        #region Helping Methods

        private ISession? createSession()
        {
            // Get HttpContext
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                // No user session could be created
                // No data could be retrieved
                // Return default Json Model
                return null;
            }

            // Return the user session
            return httpContext.Session;
        }

        private bool IsDividingByZero(string expression)
        {
            // Check expression for value
            if (string.IsNullOrEmpty(expression))
                return false;

            // Regular expression to detect division by zero
            string pattern = @"(/0+(\.0+)?)(?![0-9\.])";
            return Regex.IsMatch(expression, pattern);
        }

        #endregion

        #region Json Objects

        private JsonResult DefaultJsonObject(Models.CalculatorModel model)
        {
            if (model == null)
            {
                // No user session could be created
                // Return default Json Model
                return DefaultJsonObject();
            }
            else
            {
                // No user session could be created
                // Return default Json Model
                return Json(new
                {
                    Expression = model.Expression,
                    Result = model.Result,
                    Calculations = new List<float>()
                });
            }
        }

        private JsonResult DefaultJsonObject()
        {
            // No user session could be created
            // Return default Json Model
            return Json(new
            {
                Expression = "",
                Result = "",
                Calculations = new List<float>()
            });
        }

        #endregion
    }
}
