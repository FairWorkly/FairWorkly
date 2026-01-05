# FairWorkly 完整交互测试清单

## 📁 文件清单

```
1. FairWorkly-HomePage-FINAL.html  (Homepage)
2. login.html                       (Login & Signup)
3. templates.html                   (CSV Templates)
```

---

## ✅ 测试1: Homepage → Login (登录)

### 步骤：

1. 打开 `FairWorkly-HomePage-FINAL.html`
2. 点击 Navbar 右上角 "Log In" 按钮

### 预期结果：

✅ 跳转到 `login.html`
✅ "Sign In" tab 激活（默认）
✅ 显示登录表单：

- Email
- Password
- Remember me checkbox
- Forgot password? 链接

---

## ✅ 测试2: Homepage → Create Account (注册)

### 步骤：

1. 打开 `FairWorkly-HomePage-FINAL.html`
2. 点击以下任一按钮：
   - Navbar "Start Free Trial"
   - Hero Section "Start Free Trial"
   - Pricing Starter "Start Free Trial"
   - Pricing Professional "Start Free Trial"

### 预期结果：

✅ 跳转到 `login.html?signup=true`
✅ "Create Account" tab 自动激活
✅ 显示注册表单字段（按顺序）：

1.  Full Name
2.  Company Name
3.  Work Email
4.  Password (带强度检查)
5.  Number of Employees (下拉菜单)
    - Select range
    - 1-50 employees
    - 51-150 employees
    - 150+ employees
6.  提示文字："This helps us recommend the right plan for you"

---

## ✅ 测试3: Login → Homepage (返回)

### 步骤：

1. 在 `login.html` 页面
2. 点击左侧品牌区域的 "FairWorkly" Logo

### 预期结果：

✅ 返回 `FairWorkly-HomePage-FINAL.html`
✅ 滚动到页面顶部

---

## ✅ 测试4: Homepage → CSV Templates

### 步骤：

1. 打开 `FairWorkly-HomePage-FINAL.html`
2. 滚动到 FAQ section
3. 找到 "Do you provide CSV templates?" 问题
4. 点击 "View All CSV Templates" 按钮

**或者：**

1. 滚动到 Footer
2. 在 Resources 列找到 "CSV Templates"
3. 点击

### 预期结果：

✅ 跳转到 `templates.html`
✅ 显示 Coming Soon 页面
✅ 看到内容：

- 大下载图标
- "Coming Soon" badge
- "CSV Templates" 标题
- 说明文字
- "Request Early Access" 按钮
- "What's Coming" 列表

---

## ✅ 测试5: Templates → Homepage (返回)

### 步骤：

1. 在 `templates.html` 页面
2. 滚动到底部
3. 点击 "Back to Home" 链接

### 预期结果：

✅ 返回 `FairWorkly-HomePage-FINAL.html`
✅ 滚动到页面顶部

---

## ✅ 测试6: Contact Sales Modal

### 步骤：

1. 打开 `FairWorkly-HomePage-FINAL.html`
2. 滚动到 Pricing section
3. 找到 Enterprise plan (第3个pricing card)
4. 点击 "Contact Sales" 按钮

### 预期结果：

✅ Modal 立即弹出（不跳转页面）
✅ 背景变暗（半透明黑色遮罩）
✅ Modal 内容：

- 大邮件图标（紫色）
- "Contact Sales" 标题
- "Interested in Enterprise? Let's talk!"
- "Email us at:"
- **support@fairworkly.com** (可见，可点击)
- "We typically respond within 24 hours."

### 测试关闭Modal：

**方式1: 点击 X**

1. 点击 Modal 右上角 X
2. ✅ Modal 关闭

**方式2: 点击外部**

1. 点击 Modal 外部黑色遮罩区域
2. ✅ Modal 关闭

**方式3: ESC键**

1. 按键盘 ESC 键
2. ✅ Modal 关闭

### 测试邮箱链接：

1. 点击 Modal 中的 `support@fairworkly.com`
2. ✅ 打开邮件客户端
3. ✅ 收件人自动填充为 support@fairworkly.com

---

## ✅ 测试7: Footer 邮箱链接

### 步骤：

1. 打开 `FairWorkly-HomePage-FINAL.html`
2. 滚动到 Footer
3. 在左侧品牌区域找到 "Support:" 邮箱

### 预期结果：

✅ 看到 "Support: support@fairworkly.com"
✅ 邮箱是紫色（可点击）
✅ 点击后打开邮件客户端

### 步骤（Contact链接）：

1. 在 Footer "Product" 列
2. 点击 "Contact" 链接

### 预期结果：

✅ 打开邮件客户端
✅ 收件人为 support@fairworkly.com

---

## ✅ 测试8: Login Tab 切换

### 步骤：

1. 打开 `login.html`（默认 Sign In tab）
2. 点击 "Create Account" tab

### 预期结果：

✅ 切换到 Create Account 表单
✅ URL 不改变

### 步骤（反向）：

1. 打开 `login.html?signup=true`（Create Account tab激活）
2. 点击 "Sign In" tab

### 预期结果：

✅ 切换到 Sign In 表单
✅ URL 保持 ?signup=true（不影响功能）

---

## ✅ 测试9: 平滑滚动

### 步骤：

1. 打开 `FairWorkly-HomePage-FINAL.html`
2. 点击 Hero Section "View Pricing" 按钮

### 预期结果：

✅ 页面平滑滚动到 Pricing section
✅ 不跳转页面

### 其他锚点测试：

- Navbar "Features" → ✅ 滚动到 Features
- Navbar "Pricing" → ✅ 滚动到 Pricing
- Navbar "FAQ" → ✅ 滚动到 FAQ

---

## ✅ 测试10: Create Account 表单验证

### 步骤：

1. 打开 `login.html?signup=true`
2. 尝试提交空表单

### 预期结果：

✅ 浏览器显示 "Please fill out this field" 提示

### 密码强度测试：

1. 在 Password 字段输入：`abc`
2. ✅ 强度条显示弱（红色）

3. 输入：`Password123`
4. ✅ 强度条显示中等（橙色/黄色）

5. 输入：`P@ssw0rd123!`
6. ✅ 强度条显示强（绿色）

---

## 📊 最终检查清单

### Homepage (FairWorkly-HomePage-FINAL.html)

```
✅ Navbar: Log In → login.html
✅ Navbar: Start Free Trial → login.html?signup=true
✅ Hero: Start Free Trial → login.html?signup=true
✅ Hero: View Pricing → #pricing (平滑滚动)
✅ Pricing Starter: Start Free Trial → login.html?signup=true
✅ Pricing Professional: Start Free Trial → login.html?signup=true
✅ Pricing Enterprise: Contact Sales → Modal弹出
✅ FAQ: View All CSV Templates → templates.html
✅ Footer: CSV Templates → templates.html
✅ Footer: Support邮箱 → 可见，可点击
✅ Footer: Contact → mailto:support@fairworkly.com
✅ Modal: 可打开，可关闭，邮箱可点击

```

### Login (login.html)

```
✅ Logo → FairWorkly-HomePage-FINAL.html
✅ Sign In tab → 登录表单
✅ Create Account tab → 注册表单
✅ ?signup=true → 自动激活Create Account tab
✅ Number of Employees: 3个选项
✅ 密码强度检查工作正常
✅ 左侧品牌文案优化（无假数据）
```

### Templates (templates.html)

```
✅ Back to Home → FairWorkly-HomePage-FINAL.html
✅ Request Early Access → mailto:support@fairworkly.com
✅ Coming Soon页面完整显示
```

---

## 🎯 交互流程图

```
┌──────────────────────────────────────┐
│  FairWorkly-HomePage-FINAL.html      │
│                                      │
│  [Log In] ──────────────┐            │
│  [Start Free Trial] ─┐  │            │
│  [View Pricing] ─────┼──┼─→ #pricing │
│  [Contact Sales] ────┼──┼─→ Modal    │
│  [CSV Templates] ────┼──┼───────┐    │
└──────────────────────┼──┼───────┼────┘
                       │  │       │
                       ↓  ↓       ↓
              ┌────────────┐  ┌──────────┐
              │ login.html  │  │templates │
              │             │  │  .html   │
              │ [Logo] ─────┼─→│[Back]───→│
              │  → Home     │  │  → Home  │
              └────────────┘  └──────────┘
```

---

## 🚀 全部测试通过后

如果所有测试都通过，说明：

1. ✅ Homepage和Login完全交互
2. ✅ Homepage和Templates完全交互
3. ✅ Contact Sales Modal完全工作
4. ✅ Create Account表单是最新版本
5. ✅ 所有邮箱链接都正确

**可以开始开发后端了！** 🎉
