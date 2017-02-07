﻿using System.Collections.Generic;
using VisioAutomation.Exceptions;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioAutomation.ShapeSheet
{
    public struct ShapeSheetSurface
    {
        public readonly SurfaceTarget Target;

        public ShapeSheetSurface(SurfaceTarget target)
        {
            this.Target = target;
        }

        public ShapeSheetSurface(IVisio.Page page)
        {
            this.Target = new SurfaceTarget(page);
        }

        public ShapeSheetSurface(IVisio.Master master)
        {
            this.Target = new SurfaceTarget(master);
        }

        public ShapeSheetSurface(IVisio.Shape shape)
        {
            this.Target = new SurfaceTarget(shape);
        }

        public int SetFormulas(short[] stream, object[] formulas, short flags)
        {
            if (this.Target.Shape != null)
            {
                return this.Target.Shape.SetFormulas(stream, formulas, flags);
            }
            else if (this.Target.Master != null)
            {
                return this.Target.Master.SetFormulas(stream, formulas, flags);
            }
            else if (this.Target.Page != null)
            {
                return this.Target.Page.SetFormulas(stream, formulas, flags);
            }

            throw new System.ArgumentException("Unhandled Target");
        }

        public int SetResults(short[] stream, object[] unitcodes, object[] results, short flags)
        {
            if (this.Target.Shape != null)
            {
                return this.Target.Shape.SetResults(stream, unitcodes, results, flags);
            }
            else if (this.Target.Master != null)
            {
                return this.Target.Master.SetResults(stream, unitcodes, results, flags);
            }
            else if (this.Target.Page != null)
            {
                return this.Target.Page.SetResults(stream, unitcodes, results, flags);
            }

            throw new System.ArgumentException("Unhandled Target");
        }

        public TResult[] _GetResults<TResult>(short[] stream, IList<IVisio.VisUnitCodes> unitcodes, int numitems)
        {
            var surface = this;

            EnforceValidResultType(typeof(TResult));

            if (numitems < 1)
            {
                return new TResult[0];
            }

            var result_type = typeof(TResult);
            var unitcodes_obj_array = get_unit_code_obj_array(unitcodes);
            var flags = get_VisGetSetArgs(result_type);

            System.Array results_sa = null;

            if (surface.Target.Master != null)
            {
                surface.Target.Master.GetResults(stream, (short)flags, unitcodes_obj_array, out results_sa);
            }
            else if (surface.Target.Page != null)
            {
                surface.Target.Page.GetResults(stream, (short)flags, unitcodes_obj_array, out results_sa);
            }
            else if (surface.Target.Shape != null)
            {
                surface.Target.Shape.GetResults(stream, (short)flags, unitcodes_obj_array, out results_sa);
            }
            else
            {
                throw new System.ArgumentException("Unhandled Target");
            }

            if (results_sa.Length != numitems)
            {
                string msg = string.Format("Expected {0} items from GetResults but only received {1}", numitems,
                    results_sa.Length);
                throw new InternalAssertionException(msg);
            }

            var results = new TResult[results_sa.Length];
            results_sa.CopyTo(results, 0);
            return results;
        }

        private static void EnforceValidResultType(System.Type result_type)
        {
            if (!IsValidResultType(result_type))
            {
                string msg = string.Format("Unsupported Result Type: {0}", result_type.Name);
                throw new InternalAssertionException(msg);
            }
        }

        private static bool IsValidResultType(System.Type result_type)
        {
            return (result_type == typeof(int)
                    || result_type == typeof(double)
                    || result_type == typeof(string));
        }

        private static IVisio.VisGetSetArgs get_VisGetSetArgs(System.Type type)
        {
            IVisio.VisGetSetArgs flags;
            if (type == typeof(int))
            {
                flags = IVisio.VisGetSetArgs.visGetTruncatedInts;
            }
            else if (type == typeof(double))
            {
                flags = IVisio.VisGetSetArgs.visGetFloats;
            }
            else if (type == typeof(string))
            {
                flags = IVisio.VisGetSetArgs.visGetStrings;
            }
            else
            {
                string msg = string.Format("Unsupported Result Type: {0}", type.Name);
                throw new InternalAssertionException(msg);
            }
            return flags;
        }

        private static object[] get_unit_code_obj_array(IList<IVisio.VisUnitCodes> unitcodes)
        {
            // Create the unit codes array
            object[] unitcodes_obj_array = null;
            if (unitcodes != null)
            {
                unitcodes_obj_array = new object[unitcodes.Count];
                for (int i = 0; i < unitcodes.Count; i++)
                {
                    unitcodes_obj_array[i] = unitcodes[i];
                }
            }
            return unitcodes_obj_array;
        }

        private string[] _GetFormulasU(short[] stream, int numitems)
        {
            var surface = this;

            if (numitems < 1)
            {
                return new string[0];
            }

            System.Array formulas_sa = null;

            if (surface.Target.Master != null)
            {
                surface.Target.Master.GetFormulasU(stream, out formulas_sa);
            }
            else if (surface.Target.Page != null)
            {
                surface.Target.Page.GetFormulasU(stream, out formulas_sa);
            }
            else if (surface.Target.Shape != null)
            {
                surface.Target.Shape.GetFormulasU(stream, out formulas_sa);
            }
            else
            {
                throw new System.ArgumentException("Unhandled Drawing Surface");
            }

            object[] formulas_obj_array = (object[])formulas_sa;

            if (formulas_obj_array.Length != numitems)
            {
                string msg = string.Format("Expected {0} items from GetFormulas but only received {1}", numitems,
                    formulas_obj_array.Length);
                throw new InternalAssertionException(msg);
            }

            string[] formulas = new string[formulas_obj_array.Length];
            formulas_obj_array.CopyTo(formulas, 0);
            return formulas;
        }

        private static int check_stream_size(short[] stream, int chunksize)
        {
            if ((chunksize != 3) && (chunksize != 4))
            {
                throw new System.ArgumentOutOfRangeException("Chunksize must be 3 or 4");
            }

            int remainder = stream.Length % chunksize;

            if (remainder != 0)
            {
                string msg = string.Format("stream must have a multiple of {0} elements", chunksize);
                throw new System.ArgumentException(msg);
            }

            return stream.Length / chunksize;
        }

        public string[] GetFormulasU_SIDSRC(short[] stream)
        {
            int numitems = check_stream_size(stream, 4);
            var formulas = this._GetFormulasU(stream, numitems);
            return formulas;
        }

        public string[] GetFormulasU_SRC(short[] stream)
        {
            int numitems = check_stream_size(stream, 3);
            var formulas = this._GetFormulasU(stream, numitems);
            return formulas;
        }

        public TResult[] GetResults_SIDSRC<TResult>(short[] stream, IList<IVisio.VisUnitCodes> unitcodes)
        {
            int numitems = check_stream_size(stream, 4);
            var results = this._GetResults<TResult>(stream, unitcodes, numitems);
            return results;
        }

        public TResult[] GetResults_SRC<TResult>(short[] stream, IList<IVisio.VisUnitCodes> unitcodes)
        {
            int numitems = check_stream_size(stream, 3);
            var results = this._GetResults<TResult>(stream, unitcodes, numitems);
            return results;
        }
    }
}