using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Common.SongAnalizer
{
    public class MusicAnalizerConfiguration : IReadOnlyList<MusicAnalizerConfiguration.ConfigurationElement>
    {
        public MusicAnalizerConfiguration(IEnumerable<ConfigurationElement> configurations)
        {
            this.configurations = configurations.ToArray();
        }

        public MusicAnalizerConfiguration(params ConfigurationElement[] configurations) : this((IEnumerable<ConfigurationElement>)configurations) { }


        private readonly ConfigurationElement[] configurations;

        public ConfigurationElement this[int index] => configurations[index];

        public int Count => configurations.Length;

        public IEnumerator<ConfigurationElement> GetEnumerator() => configurations.OfType<ConfigurationElement>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => configurations.GetEnumerator();

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract class ConfigurationElement
        {
            internal ConfigurationElement(string lable)
            {
                Lable = lable;
            }
            public string Lable { get; }
        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract class MinMaxConfigurationElement<T> : ConfigurationElement where T : IComparable<T>
        {
            internal MinMaxConfigurationElement(string lable, T min, T max) : base(lable)
            {
                this.Minimum = min;
                this.Maximum = max;
            }


            private T value;
            public T Value
            {
                get
                {
                    return value;
                }
                set
                {
                    if (Minimum.CompareTo(value) > 0)
                        this.value = Minimum;
                    else if (Maximum.CompareTo(value) < 0)
                        this.value = Maximum;
                    else
                        this.value = value;
                }
            }
            public T Minimum { get; }
            public T Maximum { get; }
        }

        public sealed class SelectionConfiguration : ConfigurationElement
        {
            internal SelectionConfiguration(string lable, IEnumerable<object> choices) : base(lable)
            {
                this.Choices = new ReadOnlyCollection<object>(choices.ToList());
            }

            public IReadOnlyList<object> Choices { get; }

            public object SelectedObject => selection == -1 ? null : Choices[selection];

            private int selection;

            public int Selection
            {
                get { return selection; }
                set
                {
                    if (value < -1 || value >= Choices.Count)
                        return;
                    selection = value;
                }
            }
        }

        public sealed class IntConfiguration : MinMaxConfigurationElement<int>
        {
            public IntConfiguration(string lable, int min, int max) : base(lable, min, max)
            {
            }
        }

        public sealed class TimeSpanConfiguration : MinMaxConfigurationElement<TimeSpan>
        {
            public TimeSpanConfiguration(string lable, TimeSpan min, TimeSpan max) : base(lable, min, max)
            {
            }
        }
        public sealed class DoubleConfiguration : MinMaxConfigurationElement<double>
        {
            public DoubleConfiguration(string lable, double min, double max) : base(lable, min, max)
            {
            }
        }
    }

}
