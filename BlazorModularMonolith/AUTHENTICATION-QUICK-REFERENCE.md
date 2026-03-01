# Quick Reference - Authentication & Navigation

## 🎯 What You'll See Now

### Home Page (`https://localhost:7189`)

#### Not Logged In ❌
```
┌─────────────────────────────────────────────┐
│ Address Management    [Login] [Register]    │
├─────────────────────────────────────────────┤
│                                             │
│     Welcome to Address Management! 🎉       │
│                                             │
│     Please login or create an account       │
│                                             │
│     ┌──────────┐  ┌──────────┐            │
│     │  LOGIN   │  │ REGISTER │            │
│     └──────────┘  └──────────┘            │
│                                             │
│     Demo Credentials:                       │
│     • admin / Admin123!                     │
│     • user / User123!                       │
│                                             │
└─────────────────────────────────────────────┘
```

#### Logged In ✅
```
┌─────────────────────────────────────────────┐
│ Address Management    👤 admin [Logout]     │
├─────────────────────────────────────────────┤
│                                             │
│     Welcome back, admin! 👋                 │
│                                             │
│     ┌───────────┐   ┌───────────┐         │
│     │    👥     │   │    🏢     │         │
│     │  PEOPLE   │   │ BUSINESSES│         │
│     └───────────┘   └───────────┘         │
│                                             │
│     Session expires: [timestamp]            │
│                                             │
└─────────────────────────────────────────────┘
```

### Navigation Menu

#### Not Logged In ❌
```
┌─────────────┐
│  🏠 Home    │
│  🔐 Login   │
│  👤 Register│
└─────────────┘
```

#### Logged In ✅
```
┌──────────────┐
│  🏠 Home     │
│  👥 People   │
│  🏢 Businesses│
└──────────────┘
```

## 🔑 How to Access Everything

### First Time Users

1. **Start at Home** → `https://localhost:7189`
2. **Click "Register"** (on home page, nav menu, or top-right)
3. **Fill form**:
   - Username: `yourname`
   - Email: `your@email.com`
   - Password: `YourPass123!`
   - Confirm Password: `YourPass123!`
4. **Submit** → Auto-logged in → See authenticated home ✅

### Existing Users

1. **Start at Home** → `https://localhost:7189`
2. **Click "Login"** (on home page, nav menu, or top-right)
3. **Enter credentials**:
   - Username: `admin`
   - Password: `Admin123!`
4. **Submit** → See authenticated home ✅

### Access Features

#### People Management
- **From Home**: Click "Go to People" card
- **From Nav Menu**: Click "People"
- **Direct URL**: `https://localhost:7189/people` (requires login)

#### Business Management
- **From Home**: Click "Go to Businesses" card
- **From Nav Menu**: Click "Businesses"
- **Direct URL**: `https://localhost:7189/businesses` (requires login)

## 🚫 What Happens If Not Logged In

### Try to Access Protected Pages
```
User tries: https://localhost:7189/people
   ↓
Not authenticated detected
   ↓
Redirect to: https://localhost:7189/login
   ↓
After login: Access granted ✅
```

### Navigation Menu
- People and Businesses links **hidden**
- Login and Register links **visible**

### Home Page
- Login/Register buttons **prominent**
- Feature cards **not shown**
- Demo credentials **displayed**

## 🎨 Visual Indicators

### Top-Right Corner
| State | Display |
|-------|---------|
| Not Logged In | `[Login] [Register]` |
| Logged In | `👤 username [Logout]` |

### Side Menu
| State | Links |
|-------|-------|
| Not Logged In | Home, Login, Register |
| Logged In | Home, People, Businesses |

### Home Page
| State | Content |
|-------|---------|
| Not Logged In | Welcome message + Login/Register CTA |
| Logged In | Personalized welcome + Feature cards |

## 📋 Testing Checklist

### Basic Flow
- [ ] Open `https://localhost:7189`
- [ ] See Login/Register buttons
- [ ] Click Register → Fill form → Success
- [ ] See authenticated home page
- [ ] See username in top-right
- [ ] See People/Businesses in nav
- [ ] Click "Go to People" → Loads successfully
- [ ] Click "Go to Businesses" → Loads successfully
- [ ] Click Logout → Redirected to login

### Protected Routes
- [ ] Logout
- [ ] Try URL: `https://localhost:7189/people`
- [ ] Should redirect to login
- [ ] Login
- [ ] Try URL: `https://localhost:7189/people`
- [ ] Should show People page ✅

### Session Persistence
- [ ] Login
- [ ] Navigate to People
- [ ] Press F5 (refresh)
- [ ] Still logged in ✅
- [ ] Data still loads ✅

## 🎯 Default Accounts

| Username | Password | Roles | Access |
|----------|----------|-------|--------|
| admin | Admin123! | Admin, User | Full access |
| user | User123! | User | Full access |

## 🆘 Troubleshooting

### "I don't see Login/Register buttons"
**Check**: Top-right corner and home page - they should be visible

**Fix**: Clear browser cache, restart app

### "People/Businesses links not showing after login"
**Check**: Did login succeed? See username in top-right?

**Fix**: Logout, clear localStorage (F12 → Application → localStorage), login again

### "Redirected to login when accessing /people"
**Expected**: This is correct behavior when not logged in

**Solution**: Login first, then access People

### "Can't create people/businesses"
**Check**: Are you logged in? See username in top-right?

**Fix**: See "AUTHENTICATION-ISSUES-FIXED.md" for token setup

## 📖 Documentation

- **AUTHENTICATION-SETUP.md** - Initial setup details
- **AUTHENTICATION-UI-IMPROVEMENTS.md** - UI changes explained
- **AUTHENTICATION-ISSUES-FIXED.md** - Token management fixes
- **AUTHENTICATION-TESTING.md** - Detailed testing guide
