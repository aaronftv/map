using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CalculatorChallenge
{
    public partial class Default : System.Web.UI.Page
    {
        Calculator calc = new Calculator();
        /// <summary>
        /// Page_Load event
        /// </summary>
        /// <param name="sender">Sender parameter</param>
        /// <param name="e">e parameter</param>
        protected void Page_Load(object sender, EventArgs e)
        {  
        }

        /// <summary>
        /// calculateBtn_Click event
        /// </summary>
        /// <param name="sender">Sender parameter</param>
        /// <param name="e">e parameter</param
        protected void calculateBtn_Click(object sender, EventArgs e)
        {
            string input = inputTbx.Text;
            inputTbx.Text = calc.CalculateExpression(input);
        }
    }
}