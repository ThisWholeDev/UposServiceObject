﻿using System;
using Upos.ServiceObject.Base;
using Upos.ServiceObject.CashDrawer.Interfaces;

namespace Upos.ServiceObject.CashDrawer
{
    public abstract class CashDrawerBaseServiceObject : UposBase, ICashDrawer
    {
        private readonly ICashDrawerDevice _device;

        protected CashDrawerBaseServiceObject(ICashDrawerDevice device)
            : base(new CashDrawerProperties(), device)
        {
            _device = device;
        }

        public int OpenDrawer()
        {
            //Assert Drawer Enabled
            try
            {
                _device.OpenDrawer();
                if(_device.CanReportStatus)
                {
                    //var opened = _device.GetStatus();
                    //if (openend) {
                    var opened = new StatusEvents.CashDrawerOpened();
                    //_dispatcher.SendStatusUpdate(opened); }
                }

                return (int)ResultCodeConstants.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (int)ResultCodeConstants.Failure; //TODO get error codes
            }
        }

        public int WaitForDrawerClose(int beepTimeout, int beepFrequency, int beepDuration, int beepDelay)
        {
            //Assert Drawer Enabled
            try
            {
                _device.WaitForDrawerClose();
                if (_device.CanReportStatus)
                {
                    //var opened = _device.GetStatus();
                    //if (closed) {
                    var closed = new StatusEvents.CashDrawerClosed();
                    //_dispatcher.SendStatusUpdate(closed); }
                }

                return (int)ResultCodeConstants.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (int)ResultCodeConstants.Failure; //TODO get error codes
            }
        }

        protected override COPOS GetDeviceSpecificControlObjectDispatcher(object dispatchObject)
        {
            return new CashDrawerControlObject(dispatchObject);
        }
    }
}