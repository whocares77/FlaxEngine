// Copyright (c) 2012-2021 Wojciech Figat. All rights reserved.

using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Timeline.GUI
{
    /// <summary>
    /// The timeline background area control.
    /// </summary>
    class BackgroundArea : Panel
    {
        private Timeline _timeline;
        internal bool _rightMouseButtonDown;
        private Vector2 _rightMouseButtonLastPos;
        private float _rightMouseButtonMovement;

        public BackgroundArea(Timeline timeline)
        : base(ScrollBars.Both)
        {
            _timeline = timeline;
        }

        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton button)
        {
            if (base.OnMouseDown(location, button))
                return true;

            if (button == MouseButton.Right)
            {
                _rightMouseButtonDown = true;
                _rightMouseButtonLastPos = location;
                _rightMouseButtonMovement = 0;
                Focus();
                StartMouseCapture();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override void OnMouseMove(Vector2 location)
        {
            // Panning timeline view with a right-mouse button
            if (_rightMouseButtonDown)
            {
                var movePos = location + ViewOffset;
                var delta = _rightMouseButtonLastPos - movePos;
                _rightMouseButtonLastPos = movePos;
                _rightMouseButtonMovement += delta.Length;

                var hScroll = HScrollBar.Visible && HScrollBar.Enabled;
                var vScroll = VScrollBar.Visible && VScrollBar.Enabled;
                if (vScroll && hScroll)
                    Cursor = CursorType.SizeAll;
                else if (vScroll)
                    Cursor = CursorType.SizeNS;
                else if (hScroll)
                    Cursor = CursorType.SizeWE;

                bool wasLocked = IsLayoutLocked;
                IsLayoutLocked = true;
                if (hScroll)
                    HScrollBar.TargetValue += delta.X;
                if (vScroll)
                    VScrollBar.TargetValue += delta.Y;
                IsLayoutLocked = wasLocked;
                PerformLayout();
                return;
            }

            base.OnMouseMove(location);
        }

        /// <inheritdoc />
        public override bool OnMouseUp(Vector2 location, MouseButton button)
        {
            if (button == MouseButton.Right && _rightMouseButtonDown)
            {
                EndMouseCapture();
                _rightMouseButtonDown = false;
                Cursor = CursorType.Default;
                if (_rightMouseButtonMovement < 1.0f)
                {
                    _timeline.ShowContextMenu(PointToParent(_timeline, location));
                }
                return true;
            }

            return base.OnMouseUp(location, button);
        }
    }
}