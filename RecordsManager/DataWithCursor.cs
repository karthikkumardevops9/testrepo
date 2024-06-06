using System;
using System.Data;
using System.Diagnostics;

namespace MSRecordsEngine.RecordsManager
{
    public class DataWithCursor : DataTable
    {

        private DataRow _newRow = null;
        private int _currentPosition = 0;

        public bool EOF
        {
            get
            {
                try
                {
                    return Rows.Count == 0 || Rows.Count <= Index;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return true;
                }
            }
        }

        private int Index
        {
            get
            {
                int IndexRet = default;
                IndexRet = _currentPosition;
                return IndexRet;
            }
            set
            {
                _currentPosition = value;
            }
        }

        public DataRow CurrentRow
        {
            get
            {
                try
                {
                    if (_newRow is not null)
                        return _newRow;
                    return Rows[Index];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public DataRow MoveFirst()
        {
            DataRow row;
            CancelEdit();

            try
            {
                row = Rows[0];
                Index = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return row;
        }

        public DataRow MoveLast()
        {
            DataRow row;
            CancelEdit();

            try
            {
                row = Rows[Rows.Count - 1];
                Index = Rows.Count - 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return row;
        }

        public DataRow MoveNext()
        {
            DataRow row;
            CancelEdit();

            try
            {
                if (Rows.Count > Index + 1)
                {
                    row = Rows[Index + 1];
                }
                else
                {
                    row = Rows[Index];
                }
                Index += 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return row;
        }

        public DataRow MovePrevious()
        {
            DataRow row;
            CancelEdit();

            try
            {
                if (Index - 1 >= 0)
                {
                    row = Rows[Index - 1];
                }
                else
                {
                    row = Rows[Index];
                }
                Index -= 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return row;
        }

        public bool IsAddRow()
        {
            return _newRow is not null;
        }

        public void AddNew()
        {
            _newRow = NewRow();

            while (Rows.Count > 0)
                Rows.RemoveAt(0);

            Rows.Add(_newRow);
        }

        public void RemoveNewRow()
        {
            CancelEdit();
        }

        private void CancelEdit()
        {
            if (_newRow is null)
                return;

            try
            {
                _newRow.CancelEdit();
                Rows.Remove(_newRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _newRow = null;
            }

        }
    }
}