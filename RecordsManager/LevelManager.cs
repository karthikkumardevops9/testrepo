using System;
using System.Collections.Generic;
using Smead.Security;

namespace MSRecordsEngine.RecordsManager
{
    public class LevelManager : ICloneable
    {
        private Passport _passport;
        public LevelManager(Passport passport)
        {
            _passport = passport;
            // _dataBaseSearchParams = New Parameters(_passport)
        }
        private int _currentLevel = -1;
        public int CurrentLevel
        {
            get
            {
                return _currentLevel;
            }
            set
            {
                _currentLevel = value;
            }
        }
        private List<Level> _levels = new List<Level>();
        public List<Level> Levels
        {
            get
            {
                return _levels;
            }
        }
        public Level ActiveLevel
        {
            get
            {
                if (_currentLevel == -1 | _currentLevel >= _levels.Count)
                    return null;
                return _levels[_currentLevel];
            }
        }
        public string LevelInformation
        {
            get
            {
                string str = "";
                str += "Table:" + _levels[_currentLevel].Parameters.TableName + ", ";
                str += "ViewID:" + _levels[_currentLevel].Parameters.ViewId + ", ";
                str += "ViewName:" + _levels[_currentLevel].Parameters.ViewName + ", ";
                if (_levels[_currentLevel].Parameters.Parameter is not null)
                {
                    str += "QryField:" + _levels[_currentLevel].Parameters.Parameter.ParentField + ", ";
                    str += "QryValue:" + _levels[_currentLevel].Parameters.Parameter.ParentValue + ", ";
                }
                else
                {
                    str += "QryField:, QryValue:, ";
                }
                return str;
            }
        }
        public Parameters DatabaseSearchParams
        {
            get
            {
                return _dataBaseSearchParams;
            }
            set
            {
                _dataBaseSearchParams = value;
            }
        }
        private Parameters _dataBaseSearchParams;
        public int UndockedWindowCount
        {
            get
            {
                return _undockedWindowCount;
            }
            set
            {
                _undockedWindowCount = value;
            }
        }
        private int _undockedWindowCount;
        public Parameters NewestRecord
        {
            get
            {
                return _newestRecord;
            }
            set
            {
                _newestRecord = value;
            }
        }
        private Parameters _newestRecord;
        public object Clone()
        {
            LevelManager newLM = (LevelManager)MemberwiseClone();
            var tempLevel = new List<Level>();
            foreach (var lev in Levels)
                tempLevel.Add((Level)lev.Clone());
            newLM._levels = tempLevel;
            return newLM;
        }
    }

    public class Level : ICloneable
    {
        private Passport _passport;
        public Level(int viewID, Passport passport)
        {
            _passport = passport;
            _parameters = new Parameters(viewID, _passport);
        }
        public Level(Passport passport)
        {
            _passport = passport;
            _parameters = new Parameters(passport);
        }
        private Parameters _parameters;
        public Parameters Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }
        private string _cursorField = "";
        public string CursorField
        {
            get
            {
                return _cursorField;
            }
            set
            {
                _cursorField = value;
            }
        }
        private string _cursorValue = "";
        public string CursorValue
        {
            get
            {
                return _cursorValue;
            }
            set
            {
                _cursorValue = value;
            }
        }
        private int _currentRow = 1;
        public int CurrentRow
        {
            get
            {
                return _currentRow;
            }
            set
            {
                _currentRow = value;
            }
        }
        private List<string> _selectedItems = new List<string>();
        public List<string> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
            }
        }
        private List<int> _selectedRows = new List<int>();
        public List<int> SelectedRows
        {
            get
            {
                return _selectedRows;
            }
            set
            {
                _selectedRows = value;
            }
        }
        public object Clone()
        {
            return (Level)MemberwiseClone();
        }
    }
}