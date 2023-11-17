1. 把GameFramework\Debugger内的类 替换掉 Debugger/Core 文件
2. 把UnityGameFramework\Scripts\Runtime\Debugger里的类 替换到 Debugger\Component
3. 删除DebuggerComponent.OperationsWindow.cs 和DebuggerComponent.ObjectPoolInformationWindow.cs
4. 打开DebuggerComponent.cs  注释掉报错的行 添加一个单例定义

		private static DebuggerComponent m_Instance;
        public static DebuggerComponent Instance
        {
            get
            {
                return m_Instance;
            }
        }
5. 在Awake函数里面把base.Awake(); 下面添加代码 

            if (null != m_Instance)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
 
