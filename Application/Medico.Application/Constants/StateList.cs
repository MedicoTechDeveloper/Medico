using System.Collections.Generic;

namespace Medico.Application.Constants
{
    public class StateList
    {
        private readonly IDictionary<int, string> _stateListDictionary = new Dictionary<int, string>
        {
            {1, "Alabama"}, {2, "Alaska"}, {3, "Arizona"}, {4, "Arkansas"},
            {5, "California"}, {6, "Colorado"}, {7, "Connecticut"}, {8, "Delaware"},
            {9, "District of Columbia"}, {10, "Florida"}, {11, "Georgia"},
            {12, "Hawaii"}, {13, "Idaho"}, {14, "Illinois"}, {15, "Indiana"},
            {16, "Iowa"}, {17, "Kansas"}, {18, "Louisiana"},
            {19, "Maine"}, {20, "Maryland"}, {21, "Massachusetts"},
            {22, "Michigan"}, {23, "Minnesota"}, {24, "Mississippi"},
            {25, "Missouri"}, {26, "Montana"}, {27, "Nebraska"}, {28, "Nevada"},
            {29, "New Hampshire"}, {30, "New Jersey"}, {31, "New Mexico"},
            {32, "New York"}, {33, "North Carolina"}, {34, "North Dakota"},
            {35, "Ohio"}, {36, "Oklahoma"}, {37, "Oregon"}, {38, "Pennsylvania"},
            {39, "Rhode Island"}, {40, "South Carolina"}, {41, "South Dakota"},
            {42, "Tennessee"}, {43, "Texas"}, {44, "Utah"}, {45, "Vermont"},
            {46, "Virginia"}, {47, "Washington"}, {48, "West Virginia"},
            {49, "Wisconsin"}, {50, "Wyoming"}
        };

        public string this[int index] => _stateListDictionary[index];
    }
}