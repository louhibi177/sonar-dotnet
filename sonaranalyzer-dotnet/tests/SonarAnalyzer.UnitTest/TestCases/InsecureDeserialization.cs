using System;
using System.Runtime.Serialization;

namespace Tests.Diagnostics
{
    [Serializable]
    public class ClassWithoutConstructorIsSafe
    {
        public string Name { get; set; }
    }

    [Serializable]
    public class ClassWithDefaultConstructorIsSafe
    {
        public string Name { get; set; }

        public ClassWithDefaultConstructorIsSafe()
        {
            Name = string.Empty;
        }
    }

    [Serializable]
    public class CtorParameterIsNotInConditional
    {
        public string Name { get; set; }

        public CtorParameterIsNotInConditional(string name)
        {
            Name = name;
        }
    }

    [Serializable]
    public class CtorParameterInIfStatement
    {
        public string Name { get; set; }

        public CtorParameterInIfStatement(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            if (string.IsNullOrEmpty(name))
                Name = name;
        }
    }

    [Serializable]
    public class CtorParameterInTernaryOperator
    {
        public string Name { get; set; }

        public CtorParameterInTernaryOperator(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            Name = string.IsNullOrEmpty(name) ? "default" : name;
        }
    }

    [Serializable]
    public class CtorParameterInCoalesceExpression
    {
        public string Name { get; set; }

        public CtorParameterInCoalesceExpression(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            Name = name ?? string.Empty;
        }
    }

    [Serializable]
    public class CtorParameterInCoalesceAssignmentExpression
    {
        public CtorParameterInCoalesceAssignmentExpression(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            name ??= string.Empty;
        }

        [Serializable]
        public class InNestedClass
        {
            public InNestedClass(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
            {
                name ??= string.Empty;
            }
        }
    }

    [Serializable]
    public class MultipleConstructorsOneUnsafe
    {
        public string Name { get; set; }

        public MultipleConstructorsOneUnsafe()
        {
        }

        public MultipleConstructorsOneUnsafe(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            Name = name ?? string.Empty;
        }

        public MultipleConstructorsOneUnsafe(string firstName, string lastName)
        {
            Name = $"{firstName} {lastName}";
        }
    }

    [Serializable]
    public class MultipleConstructorsMoreUnsafe
    {
        public string Name { get; set; }

        public MultipleConstructorsMoreUnsafe()
        {
        }

        public MultipleConstructorsMoreUnsafe(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            Name = name ?? string.Empty;
        }

        public MultipleConstructorsMoreUnsafe(string firstName, string lastName) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            Name = firstName ?? lastName;
        }
    }

    [Serializable]
    public class ParameterPartOfNestedConditional
    {
        public string Name { get; set; }

        public ParameterPartOfNestedConditional(string name) // Noncompliant {{Make sure not performing data validation after deserialization is safe here.}}
        {
            if (true)
            {
                Name = string.IsNullOrEmpty(name) ? "default" : name;
            }
        }
    }

    [Serializable]
    public class NoConditionals : ISerializable
    {
        protected NoConditionals(SerializationInfo info, StreamingContext context)
        {
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class MissingDeserializationCtor : ISerializable
    {
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class CtorWithConditionsAndMissingDeserializationCtor : ISerializable
    {
        private string name;

        public CtorWithConditionsAndMissingDeserializationCtor(string name) // Noncompliant
        {
            this.name = name ?? string.Empty;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class MultipleConstructorsNoConditionals : ISerializable
    {
        public MultipleConstructorsNoConditionals()
        {
        }

        public MultipleConstructorsNoConditionals(bool condition, string name)
        {
        }

        protected MultipleConstructorsNoConditionals(SerializationInfo info, StreamingContext context)
        {
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class WithConditionsISerializable : ISerializable
    {
        public string Url { get; }

        public WithConditionsISerializable(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Url = url;
            }
        }

        protected WithConditionsISerializable(SerializationInfo info, StreamingContext context)
        {
            foreach (var item in info)
            {
                if (item.Name == "Url")
                {
                    Url = (string) item.Value;
                }
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class WithDifferentConditionsISerializable : ISerializable
    {
        public string FirstName { get; }

        public string LastName { get; }

        public WithDifferentConditionsISerializable(string firstName, string lastName) // Compliant - FN: ctor has conditions on different args/fields/properties than deserialization constructor
        {
            FirstName = firstName ?? "";
            LastName = lastName;
        }

        protected WithDifferentConditionsISerializable(SerializationInfo info, StreamingContext context)
        {
            FirstName = info.GetString("FirstName");
            LastName = info.GetString("LastName") ?? string.Empty;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class WithoutConditionsInDeserializationCtor : ISerializable
    {
        public string Url { get; }

        public WithoutConditionsInDeserializationCtor(string url) // Noncompliant
        {
            if (string.IsNullOrEmpty(url))
            {
                Url = url;
            }
        }

        protected WithoutConditionsInDeserializationCtor(SerializationInfo info, StreamingContext context)
        {
            Url = info.GetString("Url");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }

    [Serializable]
    public class NoConstructor : IDeserializationCallback
    {
        public void OnDeserialization(object sender)
        {
        }
    }

    [Serializable]
    public class WithoutConditionsInCtor : IDeserializationCallback
    {
        private string Name { get; }

        public WithoutConditionsInCtor()
        {
            Name = string.Empty;
        }

        public void OnDeserialization(object sender)
        {
        }
    }

    [Serializable]
    public class DifferentConditionsInCtor : IDeserializationCallback
    {
        internal string Name { get; private set; }

        public DifferentConditionsInCtor(string name) // Noncompliant
        {
            Name = name ?? string.Empty;
        }

        public void UnrelatedMethod(object sender) { Name ??= string.Empty; } // Name != OnDeserialization

        public void OnDeserialization() => Name ??= string.Empty; // Wrong number of parameters

        public void OnDeserialization(string a, string b) => Name ??= string.Empty; // wrong number of parameters

        public void OnDeserialization(object sender)
        {
        }
    }

    [Serializable]
    public class WithConditionsInBothCtorAndOnDeserialization : IDeserializationCallback
    {
        public string Name { get; private set; }

        public WithConditionsInBothCtorAndOnDeserialization(string name)
        {
            Name = name ?? string.Empty;
        }

        public void OnDeserialization(object sender)
        {
            if (sender != null)
            {
                Name = sender.ToString();
            }
        }
    }

    [Serializable]
    public class WithDifferentConditionsInBothCtorAndOnDeserialization : IDeserializationCallback
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public WithDifferentConditionsInBothCtorAndOnDeserialization(string firstName, string lastName) // Compliant - FN: ctor has conditional on different argument/property/field than OnDeserialization.
        {
            FirstName = firstName ?? string.Empty;
            lastName = lastName;
        }

        public void OnDeserialization(object sender)
        {
            FirstName = (string)sender;
            LastName = true ? "a" : "b";
        }
    }
}
