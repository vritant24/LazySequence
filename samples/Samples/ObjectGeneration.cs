using System.Collections.Generic;
using System.Linq;
using LazySequence;

namespace Samples
{
    /// <summary>
    /// Examples of Object Generation
    /// </summary>
    public class ObjectGeneration
    {
        /// <summary>
        /// An example to show creation of list of objects using
        /// LazySequence
        /// </summary>
        public static void GenerateListOfPersons()
        {
            /*
             * Consider a scenario during testing where you need a list of
             * objects matching a certain pattern. For example creating a list
             * of Persons.
             */
            IEnumerable<Person?> infinitePeople = LazySequence<Person?>.Create(
                null,
                (prev, idx) => (
                    new Person($"name_{idx}"),
                    isLastElement: false));

            // A list of Person objects with unique `Name` values
            // created on demand by our Lazy Sequence
            // [Person(name+1), Person(name_2)...Person(name_10)]
            var tenPeople = infinitePeople.Take(10).ToList();
        }

        /// <summary>
        /// An example to show generation of unique names using
        /// LazySequence
        /// </summary>
        public static void GenerateUniqueNames()
        {
            /*
             * Consider a scenario where you need to generate unique names.
             */
            IEnumerable<string> uniqueNames = LazySequence<string>.Create(
                string.Empty,
                (prev, idx) => ($"name_{idx}", false));

            // skipping the first element since the first element is an emoty string
            var uniqueNameProvider = uniqueNames.Skip(1).GetEnumerator();

            if (uniqueNameProvider.TryGetNext(out var name))
            {
                // `name` here would be unique
                // example: "name_1"
            }
        }

        private record Person(string Name);
    }
}
