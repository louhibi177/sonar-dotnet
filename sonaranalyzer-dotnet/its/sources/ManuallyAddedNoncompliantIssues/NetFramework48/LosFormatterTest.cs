﻿using System.Web.UI;

namespace NetFramework48
{
    public class LosFormatterTest
    {
        public void Constructors()
        {
            new LosFormatter(); // Noncompliant (S5773) {{Restrict types of objects allowed to be deserialized.}}

            new LosFormatter(false, ""); // Noncompliant (S5773) {{Restrict types of objects allowed to be deserialized.}}
            new LosFormatter(false, new byte[0]); // Noncompliant (S5773) {{Restrict types of objects allowed to be deserialized.}}

            new LosFormatter(true, ""); // Compliant - MAC filtering is enabled
            new LosFormatter(true, new byte[0]); // Compliant - MAC filtering is enabled
        }
    }
}
