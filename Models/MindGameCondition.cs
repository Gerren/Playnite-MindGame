using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using MindGame.GameProperties;

namespace MindGame.Models
{
    public class MindGameCondition : ObservableObject
    {
        private IMindGameProperty type;
        private Guid? id;
        private ConditionOperatorType? conditionOperator;

        public enum ConditionOperatorType
        {
            Has,
            HasNot,
            DoesNotMatter,
        }

        public IMindGameProperty Type { get => type; set => SetValue(ref type, value); }
        public Guid? Id
        {
            get => id; set
            {
                SetValue(ref id, value);
                OnPropertyChanged("Value");
            }
        }
        public ConditionOperatorType? ConditionOperator { get => conditionOperator; set => SetValue(ref conditionOperator, value); }

        public string OperatorText
        {
            get
            {
                switch(ConditionOperator)
                {
                    case ConditionOperatorType.Has: return ResourceProvider.GetString("LOCMindGameHas");
                    case ConditionOperatorType.HasNot: return ResourceProvider.GetString("LOCMindGameHasNot");
                    case ConditionOperatorType.DoesNotMatter: return ResourceProvider.GetString("LOCMindGameDoesNotMatter");
                    default: return ResourceProvider.GetString("LOCCrashWindowTitle");
                }
            }
        }
        public string Value => Id.HasValue ? type.GetValue(Id.Value) : string.Empty;
    }
}
