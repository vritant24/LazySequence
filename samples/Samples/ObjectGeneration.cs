using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// sequence
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

        private record Person(string Name);
    }
}
