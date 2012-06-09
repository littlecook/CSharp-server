using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMyCom
{
    //属性-值
    public class Attribute<T, U>
    {
        private T m_type;       //标志
        private U m_value;      //值

        public Attribute(T t, U u)
        {
            this.m_type = t;
            this.m_value = u;
        }
        public T Type
        {
            get
            {
                return m_type;
            }
        }

        public U Value
        {
            set
            {
                m_value = value;
            }
            get
            {
                return this.m_value;
            }
        }
    }

    //数据银行类:管理类型-值List
    public class DataBank<T, U>
    {
        private List<Attribute<T, U> > m_attributes = new List<Attribute<T,U>>();

        public void delAttribute(T type)
        {
            int index = 0;
            Boolean isExist = false;
            foreach (Attribute<T, U> attribute in m_attributes)
            {
                if (attribute.Type.Equals(type))
                {
                    isExist = true;
                    break;
                }

                index++;
            }
            if (isExist)
            {
                m_attributes.RemoveAt(index);
            }
        }
        public void addAttribute(T type, U data)
        {
            Attribute<T, U> find = getItem(type);
            if (find == null)
            {
                find = new Attribute<T, U>(type, data);
                m_attributes.Add(find);
            }
        }
        //设置属性type的值得为data
        public void setAttribute(T type, U data)
        {
            Attribute<T, U> find = null;
            foreach (Attribute<T, U> attribute in m_attributes)
            {
                if (attribute.Type.Equals(type))
                {
                    find = attribute;
                    break;
                }
            }
            if (find != null)
            {
                find.Value = data;
            }
        }

        //返回一个type指定的元素
        public Attribute<T, U> getItem(T type)
        {
            foreach (Attribute<T, U> attribute in m_attributes)
            {
                if (attribute.Type.Equals(type))
                {
                    return attribute;
                }
            }
            return null;
        }
        //获取type对应的值
        public U getAttribute(T type)
        {
            foreach (Attribute<T, U> attribute in m_attributes)
            {
                if (attribute.Type.Equals(type))
                {
                    return attribute.Value;
                }
            }
            return default(U);
        }

        //查询是否含有type属性
        public Boolean hadAttribute(T type)
        {
            foreach (Attribute<T, U> attribute in m_attributes)
            {
                if (attribute.Type.Equals(type))
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Logic
    {
        public virtual Logic clone()
        {
            return null;
        }
        public virtual void excute()
        {

        }
    }

    class LogicCollection<T>        //T代表标识
    {
        protected DataBank<T, Logic> m_collection = null;

        public void addLogic(T t, Logic logic)
        {
            m_collection.addAttribute(t,logic);
        }
        public Logic getLogic(T t)
        {
            return m_collection.getAttribute(t);
        }

        public Boolean hadLogic(T t)
        {
            return m_collection.hadAttribute(t);
        }
        public void delLogic(T t)
        {
            m_collection.delAttribute(t);
        }
    }

    class Entity
    {
        protected int m_id;
    }
}
