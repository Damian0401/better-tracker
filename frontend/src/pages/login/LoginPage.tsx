import { useState } from "react";
import { Link, useNavigate } from "@tanstack/react-router";
import type { paths } from "@/libs/api.schema.g";
import { Api } from "@/libs/api";
import { Auth } from "@/libs/auth";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { toast } from "sonner";
import { Routes } from "@/constants";

export function LoginPage() {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    login: "",
    password: "",
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const response = await Api.POST("/api/v1/auth/login", {
        body: formData,
      });

      if (response.data) {
        Auth.setToken(response.data.token);
        Auth.setUser({
          userId: response.data.userId,
          userName: response.data.userName,
        });
        navigate({ to: Routes.HOME });
      } else {
        const error = response.error as paths["/api/v1/auth/login"]["post"]["responses"] | undefined;
        const errors = error && "errors" in error ? (error as { errors: string[] }).errors : [];
        toast.error(errors.join(", ") || "Login failed");
      }
    } catch {
      toast.error("An unexpected error occurred");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-4">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Login</CardTitle>
          <CardDescription>
            Enter your credentials to access your account
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Input
                type="text"
                placeholder="Login"
                value={formData.login}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, login: e.target.value }))
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Input
                type="password"
                placeholder="Password"
                value={formData.password}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, password: e.target.value }))
                }
                required
              />
            </div>
            <Button type="submit" className="w-full" disabled={isLoading}>
              {isLoading ? "Logging in..." : "Login"}
            </Button>
          </form>
          <p className="mt-4 text-center text-sm text-muted-foreground">
            Don't have an account?{" "}
            <Link to={Routes.REGISTER} className="text-primary hover:underline">
              Register
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}